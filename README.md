# VoucherSystem
A VoucherSystem for TShock, SEconomy plugin required for it to work.

## Permission
- `vsystem.use` Permission to use the VoucherSystem plugin.
- `vsystem.admin` Permission to use the plugin's Admin commands.

## Commands
- `/voucher [parameters]` `Permission Required : vsystem.use`  Main command.

## Parameters
- `claim <serialnumber>` `Permission Required : vsystem.use`  Claim an active voucher.
- `add <serialnumber> <rewardamount> <expiration>` `Permission Required : vsystem.admin`  Create a Voucher.
- `del <voucherid>` `Permission Required : vsystem.admin`  Delete an active and claimed voucher.
- `list` `Permission Required : vsystem.admin` Show active vouchers list.

## Example
- `/voucher add myserialnumber 30000 3d` This command will try to create a voucher with serial number "myserialnumber", a reward of 30000 SEconomy money, and will be expired in 3 days.
- `/voucher claim myserialnumber` This command will try to claim a voucher with a serial number of "myserialnumber".
- `/voucher del 2` This command will try to delete a voucher in the database where the voucher id of 2.
- `/voucher list` This command will show all the active vouchers in the database.
