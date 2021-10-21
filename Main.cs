using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace VoucherSystem
{
    [ApiVersion(2, 1)]
    public class VoucherSystem : TerrariaPlugin
    {
        public override string Author => "hdseventh";
        public override string Description => "Voucher system for TShock";
        public override string Name => "Voucher System";
        public override Version Version { get { return new Version(1, 0, 0, 0); } }
        public VoucherSystem(Main game) : base(game) { }

        private DBManager vsystem;
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        public void OnInitialize(EventArgs args)
        {
            vsystem = new DBManager();
            Commands.ChatCommands.Add(new Command("vsystem.use", VoucherSystemUse, "voucher"));
        }

        async void VoucherSystemUse(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count < 1)
            {
                args.Player.SendInfoMessage("{0}voucher claim <serialnumber> - Claim a voucher.", Commands.Specifier);
                if (args.Player.HasPermission("vsystem.admin"))
                {
                    args.Player.SendInfoMessage("{0}voucher add <serialnumber> <reward> <expiration> - Add a voucher.", Commands.Specifier);
                    args.Player.SendInfoMessage("{0}voucher del <voucherid> - Delete a voucher.", Commands.Specifier);
                    args.Player.SendInfoMessage("{0}voucher list - Shows the voucher list.", Commands.Specifier);
                }
                return;
            }
            string subcmd = args.Parameters[0].ToLower();
            switch (subcmd)
            {
                case "claim":
                    if (args.Parameters.Count < 2)
                    {
                        player.SendErrorMessage("Invalid syntax! Proper syntax: {0}voucher claim <serialnumber>", Commands.Specifier);
                        return;
                    }
                    VoucherSystems vlist = vsystem.ClaimVoucher(args.Parameters[1]);
                    if (vlist == null)
                    {
                        player.SendErrorMessage("Voucher code is not valid!");
                        return;
                    }

                    if (!vsystem.UpdateVoucher(vlist.Id, player.Name))
                    {
                        player.SendErrorMessage("Error while claiming the voucher. Contact Admin for more information.");
                        return;
                    }

                    if (SEconomyPlugin.Instance != null)
                    {
                        var playerBankAccount = SEconomyPlugin.Instance.GetBankAccount(player);
                        await SEconomyPlugin.Instance.WorldAccount.TransferToAsync(
                            playerBankAccount,
                            int.Parse(vlist.Reward),
                            BankAccountTransferOptions.AnnounceToReceiver,
                            "Claimed a Voucher.",
                            "Claiming a voucher with voucher ID:" + vlist.Id);
                    }

                    break;
                case "add":
                    if (args.Parameters.Count < 3)
                    {
                        player.SendErrorMessage("Invalid syntax! Proper syntax: {0}voucher add <serialnumber> <reward> <expiration>", Commands.Specifier);
                        return;
                    }
                    DateTime expiration = DateTime.MaxValue;
                    if (args.Parameters.Count > 3)
                    {
                        if (TShock.Utils.TryParseTime(args.Parameters[3], out int seconds))
                        {
                            expiration = DateTime.UtcNow.AddSeconds(seconds);
                        }
                    }

                    if (!int.TryParse(args.Parameters[2], out int result))
                    {
                        player.SendErrorMessage("Invalid reward format! Numerical-only is the proper format.");
                        return;
                    }

                    if (vsystem.AddVoucher(args.Parameters[1], args.Parameters[2], args.Player.Name, "", expiration))
                    {
                        player.SendSuccessMessage("Successfully added a new voucher.");
                    }
                    else
                    {
                        player.SendErrorMessage("Failed adding the new voucher. Check logs for more info.");
                    }
                    break;
                case "del":
                    if (args.Parameters.Count < 2)
                    {
                        player.SendErrorMessage("Invalid syntax! Proper syntax: {0}voucher del <voucherid>", Commands.Specifier);
                        return;
                    }
                    if (vsystem.DeleteVoucher(args.Parameters[1]))
                        player.SendSuccessMessage("Deleted voucher with ID {0}.", args.Parameters[1]);
                    else
                        player.SendErrorMessage("Failed deleting voucher with ID {0}. Check logs for more info.", args.Parameters[1]);
                    break;
                case "list":
                    // integrate pagination tool
                    int pagenumber;
                    if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pagenumber))
                        return;

                    // fetch only ranges from the list
                    List<VoucherSystems> list = vsystem.GetVoucherList();
                    var namelist = from vouchers in list
                                   select vouchers.SerialNumber;


                    // show data from user's specified page
                    PaginationTools.SendPage(player, pagenumber, PaginationTools.BuildLinesFromTerms(namelist),
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Vouchers List ({0}/{1}):",
                                FooterFormat = "Type {0}voucher list {{0}} for more.".SFormat(Commands.Specifier),
                                NothingToDisplayString = "There are currently no voucher listed."
                            });
                    break;
                default:
                    args.Player.SendInfoMessage("{0}voucher claim <serialnumber> - Claim a voucher.", Commands.Specifier);
                    if (args.Player.HasPermission("vsystem.admin"))
                    {
                        args.Player.SendInfoMessage("{0}voucher add <serialnumber> <reward> <expiration> - Add a voucher.", Commands.Specifier);
                        args.Player.SendInfoMessage("{0}voucher del <voucherid> - Delete a voucher.", Commands.Specifier);
                        args.Player.SendInfoMessage("{0}voucher list - Shows the voucher list.", Commands.Specifier);
                    }
                    break;
            }
        }

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(Disposing);
        }
    }
}
