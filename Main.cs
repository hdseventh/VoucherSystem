using System;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace VoucherSystem
{
	[ApiVersion(2, 1)]
	public class VoucherSystem : TerrariaPlugin
	{
		public override string Author => "hdseventh";
		public override string Description => "Voucher system for TShock";
		public override string Name => "Voucher System";
		public override Version Version{ get { return new Version(1, 0, 0, 0); } }
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

		private void VoucherSystemUse(CommandArgs args)
        {
			TSPlayer player = args.Player;
			if (args.Parameters.Count < 1)
			{
				args.Player.SendInfoMessage("{0}voucher claim <serialnumber> - Claim a voucher.", Commands.Specifier);
				if (args.Player.HasPermission("vsystem.admin"))
				{
					args.Player.SendInfoMessage("{0}voucher add - Add a voucher.", Commands.Specifier);
					args.Player.SendInfoMessage("{0}voucher del <voucherid> - Delete a voucher.", Commands.Specifier);
				}
				return;
			}

			string subcmd = args.Parameters[0].ToLower();
			switch (subcmd)
            {
				case "claim":
					//claim placeholder
					return;
				case "add":
					//add placeholder
					return;
				case "del":
					//del placeholder
					return;
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
