using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Net;
using OpenQA.Selenium.PhantomJS;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace InstackupPhontomJS
{
    class Program
    {

        private static void centerText(String text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }

        [STAThread]
        static void Main(string[] args)
        {
            int numberPhotos = 0;
            int numberPhotosInPage = 0;
            string numberfollowers = string.Empty;
            string name = string.Empty;
            string description = string.Empty;

            ProcessStartInfo process = new ProcessStartInfo("CMD.exe");

            process.StandardOutputEncoding = Encoding.GetEncoding(860);

            using (var driverPhantom = new PhantomJSDriver())
            {
                driverPhantom.Manage().Window.Maximize();

                Console.OutputEncoding = Encoding.Unicode;
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Digite o Instagram(Desbloqueado): ");                
                string insta = Console.ReadLine();

                driverPhantom.Navigate().GoToUrl("https://www.instagram.com/" + insta);

                Thread.Sleep(1000);
                Console.WriteLine("\n");

                centerText("----------------- Capturando informações----------------- \n");
                //*[@id="react-root"]/section/main/article/header/div[2]/ul/li[2]

                try
                {
                    driverPhantom.FindElementByXPath("//*[@id='react-root']/section/main/article/div/div[3]/a").Click();

                    numberPhotos = int.Parse(driverPhantom.FindElementByXPath("//*[@id='react-root']/section/main/article/header/div[2]/ul/li[1]/span/span").Text);

                    numberPhotosInPage = driverPhantom.FindElements(By.XPath("//*[@id='react-root']/section/main/article/div/div[1]/div//a")).Count;

                    numberfollowers = driverPhantom.FindElement(By.XPath("//*[@id='react-root']/section/main/article/header/div[2]/ul/li[2]")).Text;

                    try
                    {                       
                        name = driverPhantom.FindElementByXPath("//*[@id='react-root']/section/main/article/header/div[2]/div[2]/h2").Text;

                        description = driverPhantom.FindElementByXPath("//*[@id='react-root']/section/main/article/header/div[2]/div[2]/span").Text;

                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Instagram sem Nome e Descrição");                     
                    }

                   
                    Console.WriteLine("\nNome: {0}", name.Trim());
                    Console.WriteLine("\nDescrição: {0}", description.Trim());
                    Console.WriteLine("\nSeguidores: {0}", numberfollowers);
                    Console.WriteLine("\nNúmero de publicações: {0}", numberPhotos);

                    

                }
                catch (InvalidElementStateException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
              

                try
                {
                    Console.WriteLine("\n");

                    for (int i = 0; numberPhotosInPage < numberPhotos; i++)
                    {
        
                        ((IJavaScriptExecutor)driverPhantom).ExecuteScript(@"document.body.scrollTop = document.documentElement.scrollTop = 0");
                        ((IJavaScriptExecutor)driverPhantom).ExecuteScript(@"setTimeout(function() {                                                                      
                                                                       window.scrollTo(0, document.body.scrollHeight - 150)  
                                                                       }, 1000)");
                        numberPhotosInPage = driverPhantom.FindElements(By.XPath("//*[@id='react-root']/section/main/article/div/div[1]/div//a")).Count;                       
                        Console.Write("\rCarregando fotos da página. {0} de {1}. Aguarde!!! ", numberPhotosInPage, numberPhotos);
                    }

                    

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);

                }

                Console.WriteLine("\n");
                centerText("\n----------------- Reunindo URL -----------------");
                Console.WriteLine("\n");
                //var photo = driverPhantom.FindElementByXPath("//*[@id='react-root']/section/main/article/div/div[1]/div[1]/a[1]/div/div[1]/img").GetAttribute("src").Split('?').First();

                IList<string> Link = new List<string>();

                try
                {
                    TimeSpan time = TimeSpan.FromSeconds(10);

                    IList<IWebElement> Photos = driverPhantom.FindElements(By.XPath("//*[@id='react-root']/section/main/article/div/div[1]//div/a//img"));

                    int urlCount = 0;
                    foreach (var item in Photos)
                    {
                        try
                        {
                            Console.Write("\r Preparando imagens...{0} de {1}  ", urlCount, numberPhotos);
                            driverPhantom.Manage().Timeouts().ImplicitlyWait(time);
                            Link.Add(item.GetAttribute("src").Split('?')[0]);
                            urlCount++;
                        }
                        catch (StaleElementReferenceException e)
                        {
                            Console.WriteLine("Recuperando URL");
                        }

                    }

                }
                catch (WebDriverException)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Não foi possível carregar uma ou mais imagens. Por favor, tente novamente.");
                }
  

                centerText("----------------- Selecione o Diretório para Salvar as fotos -----------------");

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    centerText("----------------- Salvando Fotos -----------------");
                    Console.WriteLine("\n");
                    string diretorio = fbd.SelectedPath;
                    int total = 0;


                        foreach (var item in Link)
                        {
                            using (WebClient webClient = new WebClient())
                            {

                                Console.Write("\r Salvado fotos...{0} de {1}  ", total, numberPhotos);
                                webClient.DownloadFile(item, diretorio + "\\foto" + (total + 1) + ".png");
                                total++;

                            }


                        } 
                    

                }


                Console.ReadKey();
            }

        }
    }
}
