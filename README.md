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
