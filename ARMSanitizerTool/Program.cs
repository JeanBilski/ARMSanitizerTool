// See https://aka.ms/new-console-template for more information
using ARMSanitizerTool.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var originalJson = File.ReadAllText("template.json", Encoding.UTF8);

var encoderSettings = new TextEncoderSettings();
encoderSettings.AllowRange(UnicodeRanges.BasicLatin);
var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
var armTemplate = JsonSerializer.Deserialize<RootArmObject>(originalJson, options);

var filteredResources = armTemplate?.Resources?.Where(item => config.GetSection("TypeBlacklist").Get<List<string>>().Select(x => !Regex.IsMatch(item.Type ?? "", x)).All(y => y)).ToList();

var stats = armTemplate?.Resources?.ToLookup(x => x.Type).ToList().Select(x => (x.Key, x.Count(), !config.GetSection("TypeBlacklist").Get<List<string>>().Select(y => !Regex.IsMatch(x.Key ?? "", y)).All(z => z))).OrderByDescending(x => x.Item2);

Console.WriteLine("Stats");
Console.WriteLine("-----");
Console.WriteLine("");

var blacklistDisplay = (bool blacklisted) => blacklisted ? "Removed" : "";
foreach (var (type, number, blacklisted) in stats)
{
    Console.WriteLine($"{type} : {number}   {blacklistDisplay(blacklisted)}");
}

armTemplate.Resources = filteredResources;

var text = JsonSerializer.Serialize(armTemplate, options);
File.WriteAllText("outputTemplate.json", text);