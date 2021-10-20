$sourceFolder = "node_modules\@microsoft\signalr\dist\browser\*"
$destinationFolder = "webroot\lib\signalr"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $sourceFolder -Destination $destinationFolder