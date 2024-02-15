using Scrabble;

var ip = "0.0.0.0";
var port = 4444;
var server = new Scrabble.Server(ip, port);
await server.start();