using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading;
using System.IO;
using HtmlAgilityPack;
using DropshipListingCheckBusiness.Services;
using DropshipListingCheckBusiness.BusinessModels;
using LINQtoCSV;


namespace DropshipListingCheckBusiness.Handlers
{
    public class DropshipListingCheckHandler : IListingCheck
    {
        #region Dependent object
        private readonly IDropshipListingCheckService _dropshipListingCheckService;
        private readonly ICSVHelper _csvHelper;
        #endregion

        public DropshipListingCheckHandler(IDropshipListingCheckService dropshipListingCheckService,ICSVHelper csvHelper)
        {
            _dropshipListingCheckService = dropshipListingCheckService;
            _csvHelper = csvHelper;
        }

        public void StartCheck()
        {
            DowloadInfo();
        }

        private void DowloadInfo()
        {
            try
            {
                //var onlyonlineDataList = _dropshipListingCheckService.GetOnlyonlineData().Take(2).ToList();
                //var dealsdirectDataList = _dropshipListingCheckService.GetDealsdirectData().Take(2).ToList();
                //var onlyonlineDataList = _dropshipListingCheckService.GetOnlyonlineData();
                //var dealsdirectDataList = _dropshipListingCheckService.GetDealsdirectData();

                if (!Directory.Exists(Config.Instance.SourceFilePath))
                {
                    Directory.CreateDirectory(Config.Instance.SourceFilePath);
                }
                var oofileName = Config.Instance.SourceFilePath + Config.Instance.OODataFeedName;
                var ddfileName = Config.Instance.SourceFilePath + Config.Instance.DDDataFeedName;
                //var filePath = Config.Instance.FilePath + fileName;
                var ftpHelper = new ftp(Config.Instance.FtpHostIP, Config.Instance.FtpUserName, Config.Instance.FtpPassword);
                ftpHelper.download(Config.Instance.FtpOOFilePath + Config.Instance.OODataFeedName, Config.Instance.SourceFilePath + Config.Instance.OODataFeedName);
                ftpHelper.download(Config.Instance.FtpDDFilePath + Config.Instance.DDDataFeedName, Config.Instance.SourceFilePath + Config.Instance.DDDataFeedName);
                ftpHelper = null;

                var context = new CsvContext();
                var inputFileDescription = new CsvFileDescription() { SeparatorChar = '\t', FirstLineHasColumnNames = true, IgnoreUnknownColumns = true };


                var dropshipListingCheckResults = new List<DropshipListingCheckBM>();
                if (File.Exists(oofileName))
                {
                    var onlyonlineDataList = context.Read<OnlyonlineData>(oofileName, inputFileDescription).Where(d => !string.IsNullOrEmpty(d.SKU)).ToList();
                    var onlyonlineResultList = onlyonlineDataList.Select(data =>
                    {
                        var htmlDoc = GetHtmlDocument(new Uri(string.Format(WebsiteURL.Onlyonline, data.SKU)));
                        var hasOnlyOneMatchResult = htmlDoc.DocumentNode.SelectSingleNode("//comment()[contains(., 'JUMPTOURL')]");
                        var hasRelateResultButNotFound = htmlDoc.DocumentNode.SelectSingleNode("//script[contains(text(), 'sli_err')]");
                        var hasNoMatchResult = htmlDoc.DocumentNode.SelectSingleNode("//script[contains(text(), 'sli_noresults')]");
                        var listingAvailable = (hasOnlyOneMatchResult != null || (hasRelateResultButNotFound == null && hasNoMatchResult == null) ? true : false);
                        LogManager.Instance.Info("ExternalSKU " + data.SKU + " InternalSKU " + data.NewAimSKU + " is " + (listingAvailable ? "available" : "unavailable") + " on " + WebsiteType.Onlyonline);
                        return new DropshipListingCheckBM() { ExternalSKU = data.SKU, Website = WebsiteType.Onlyonline, ListingAvailable = listingAvailable, InternalSKU = data.NewAimSKU };
                    });

                    dropshipListingCheckResults.AddRange(onlyonlineResultList);
                }

                if (File.Exists(ddfileName))
                {
                    inputFileDescription.SeparatorChar = ',';
                    var dealsdirectDataList = context.Read<DealsdirectData>(ddfileName, inputFileDescription).Where(d => !string.IsNullOrEmpty(d.SKU)).ToList();
                    var dealsdirectResultList = dealsdirectDataList.Select(data =>
                    {
                        var htmlDoc = GetHtmlDocument(new Uri(string.Format(WebsiteURL.Dealsdirect, data.SKU)));
                        var hasNoMatchResult = htmlDoc.DocumentNode.SelectSingleNode("//comment()[contains(., 'no_results')]");
                        var listingAvailable = (hasNoMatchResult == null ? true : false);
                        LogManager.Instance.Info("ExternalSKU " + data.SKU + " InternalSKU " + data.SKU + " is " + (listingAvailable ? "available" : "unavailable") + " on " + WebsiteType.Dealsdirect);
                        return new DropshipListingCheckBM() { ExternalSKU = data.SKU, Website = WebsiteType.Dealsdirect, ListingAvailable = listingAvailable, InternalSKU = data.SKU };
                    });

                    dropshipListingCheckResults.AddRange(dealsdirectResultList);
                }
                

                var fileName=dropshipListingCheckResults.ToExcel("DropshipListingCheck_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx","Website");
                var filePath = Config.Instance.FilePath + fileName;
                if (File.Exists(filePath))
                {
                    ftpHelper = new ftp(Config.Instance.FtpHostIP, Config.Instance.FtpUserName, Config.Instance.FtpPassword);
                    ftpHelper.upload(Config.Instance.FtpFilePath + fileName, filePath);
                    ftpHelper = null;
                    LogManager.Instance.Info("File " + fileName + " has been uploaded to FTP folder" + Config.Instance.FtpFilePath);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
            }

                        
        }

        private void DowloadInfoTest()
        {
            //var htmlDoc = GetHtmlDocument(new Uri("http://search.dealsdirect.com.au/search?asug=&w=AC-WM102-7"));
            //var resultDivNode = htmlDoc.DocumentNode.DescendantsAndSelf().Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value.Contains("sli_facetbct")).FirstOrDefault();
            //var resultNumNodeTest = htmlDoc.DocumentNode.DescendantsAndSelf().Where(n => n.Name.ToLower().Equals("div")
            //    && n.Attributes.Contains("class")
            //    && n.Attributes["class"].Value.Contains("sli_facetbct")).ToList();

            //var resultNumNode = htmlDoc.DocumentNode.DescendantsAndSelf().Where(n =>
            //    n.Name.ToLower().Equals("b")
            //    &&n.ParentNode!=null
            //    &&n.ParentNode.Name.ToLower().Equals("div")
            //    &&n.ParentNode.Attributes.Contains("class")
            //    &&n.ParentNode.Attributes["class"].Value.Contains("sli_facetbct")
            //    && n.NextSibling != null
            //    && n.NextSibling.Name.ToLower().Equals("b")
            //    && n.NextSibling.InnerHtml.Equals("Pet-Cat-HSCT011")).FirstOrDefault();
            //var hasNoMatchResult = htmlDoc.DocumentNode.SelectSingleNode("//comment()[contains(., 'no_results')]");
            //if (hasNoMatchResult == null)
            //{
            //    Console.WriteLine("SKU exists");
            //}
            //else
            //{
            //    Console.WriteLine("SKU does not exist");
            //}
            //var hasRelateResultButNotFound = htmlDoc.DocumentNode.SelectSingleNode("//script[contains(text(), 'sli_err')]");
            //var hasNoMatchResult = htmlDoc.DocumentNode.SelectSingleNode("//script[contains(text(), 'sli_noresults')]");
            //FileInfo fi = new FileInfo(@"C:\Users\Administrator\Documents\Visual Studio 2012\Projects\DropshipListingCheck\DropshipListingCheck\bin\Debug\Log\InfoLog_25.07.2014.txt");
            //var read = fi.OpenText();
            //read.ReadLine();

            //var fileContent = File.ReadLines(@"C:\Jim\Temp\OODDListing\OOfeed.txt").ToList();
            //foreach (var s in fileContent)
            //{
            //    Console.WriteLine(s); 
            //}

            //using (FileStream fsSource = new FileStream(@"C:\Jim\Temp\OODDListing\OOfeed.txt",
            //FileMode.Open, FileAccess.Read))
            //{
            //    var csvSerializer = new CsvSerializer<OnlyonlineData>();
            //    csvSerializer.Separator = '\t';
            //    var list=csvSerializer.Deserialize(fsSource);
            //    foreach (var oo in list)
            //    {
            //        Console.WriteLine(oo.SKU);
            //    }
            //}
            if (!Directory.Exists(Config.Instance.SourceFilePath))
            {
                Directory.CreateDirectory(Config.Instance.SourceFilePath);
            }
            var oofileName = Config.Instance.SourceFilePath+Config.Instance.OODataFeedName;
            var ddfileName = Config.Instance.SourceFilePath + Config.Instance.DDDataFeedName;
            //var filePath = Config.Instance.FilePath + fileName;
            var ftpHelper = new ftp(Config.Instance.FtpHostIP, Config.Instance.FtpUserName, Config.Instance.FtpPassword);
            ftpHelper.download(Config.Instance.FtpOOFilePath + Config.Instance.OODataFeedName, Config.Instance.SourceFilePath + Config.Instance.OODataFeedName);
            ftpHelper.download(Config.Instance.FtpDDFilePath + Config.Instance.DDDataFeedName, Config.Instance.SourceFilePath + Config.Instance.DDDataFeedName);
            ftpHelper = null;

            var context = new CsvContext();
            var inputFileDescription = new CsvFileDescription() { SeparatorChar = '\t', FirstLineHasColumnNames = true, IgnoreUnknownColumns = true };
            //var oofileName=Directory.GetFiles(Config.Instance.SourceFilePath).Where(fn => fn.IndexOf("OOfeed") != -1).FirstOrDefault();
            //var ddfileName = Directory.GetFiles(Config.Instance.SourceFilePath).Where(fn => fn.IndexOf("newaim_datafeed") != -1).FirstOrDefault();

            if (File.Exists(oofileName))
            {
                var oolist = context.Read<OnlyonlineData>(oofileName, inputFileDescription).ToList();
                foreach (var oo in oolist)
                {
                    Console.WriteLine(oo.SKU);
                }
            }

            if (File.Exists(ddfileName))
            {
                inputFileDescription.SeparatorChar = ',';
                var ddlist = context.Read<DealsdirectData>(ddfileName, inputFileDescription).ToList();
                foreach (var dd in ddlist)
                {
                    Console.WriteLine(dd.SKU);
                }
            }

            //foreach (var dd in ddlist)
            //{
            //    Console.WriteLine(dd.SKU);
            //}

            //foreach (var oo in oolist)
            //{
            //    Console.WriteLine(oo.SKU);
            //}



            var lines = File.ReadLines(Config.Instance.LogPath+@"InfoLog_29.07.2014.txt");
            var resultList=lines.Select(s => {
                var exSKU = (s.IndexOf("InternalSKU") != -1 ? s.Substring(s.IndexOf(" ExternalSKU ") + 13, s.IndexOf(" InternalSKU ") - (s.IndexOf(" ExternalSKU ") + 13)) : s.Substring(s.IndexOf(" ExternalSKU ") + 13, s.IndexOf(" is ") - (s.IndexOf(" ExternalSKU ") + 13)));
                var inSKU = (s.IndexOf("InternalSKU") != -1 ? s.Substring(s.IndexOf(" InternalSKU ") + 13, s.IndexOf(" is ") - (s.IndexOf(" InternalSKU ") + 13)) : exSKU);
                var isAvailable = s.Substring(s.IndexOf(" is ") + 4, s.IndexOf(" on ") - (s.IndexOf(" is ") + 4));
                var website = s.Substring(s.IndexOf(" on ") + 4, s.Length - (s.IndexOf(" on ") + 4));
                return new DropshipListingCheckBM() { ExternalSKU = exSKU, ListingAvailable = (isAvailable == "available" ? true : false), Website = website,InternalSKU=inSKU };
            }).ToList();

            var fileName = resultList.ToExcel("DropshipListingCheck_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", "Website");
            var filePath = Config.Instance.FilePath + fileName;
            if (File.Exists(filePath))
            {
                ftpHelper = new ftp(Config.Instance.FtpHostIP, Config.Instance.FtpUserName, Config.Instance.FtpPassword);
                ftpHelper.upload(Config.Instance.FtpFilePath + fileName, filePath);
                ftpHelper = null;
                LogManager.Instance.Info("File " + fileName + " has been uploaded to FTP folder" + Config.Instance.FtpFilePath);
            }
            
            //Console.ReadLine();
        }

        #region Commented
        //private void WebClientDownload()
        //{

        //    for (int i = 1; i <= 2; i++)
        //    {
        //        var webClient = new WebClient();
        //        webClient.DownloadStringCompleted+=webClient_DownloadStringCompleted;
        //        webClient.DownloadStringAsync(new Uri("http://www.realestate.com.au/rent/in-vic+3101/list-"+i+"?includeSurrounding=false"));
        //    }

            
        //}

        //private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    var htmlDoc = new HtmlDocument();
        //    htmlDoc.LoadHtml(e.Result);
        //    var resultNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id,'searchResultsTbl')]").FirstOrDefault();
        //    if (resultNode != null)
        //    {
        //        CrawlerLogManager.Instance.Info(resultNode.Id);
        //    }
        //}
        #endregion

        
        private HtmlDocument GetHtmlDocument(Uri uri)
        {
            var delay = new DelayController();
            var htmlDoc = new HtmlDocument();
            delay.StarProcess();
            using (var webClient = new WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                var htmlString = webClient.DownloadString(uri);
                htmlDoc.LoadHtml(htmlString);
            }
            delay.EndProcess();
            return htmlDoc;
        }
        
        //public Func<HtmlDocument,bool>


        
    }

    
}
