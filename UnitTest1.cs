using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;


using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

new DriverManager().SetUpDriver(new ChromeConfig()); //Скачивает драйвер

namespace ShikimoriTests
{
	public class Tests
	{
		private IWebDriver driver;
		private WebDriverWait wait;

		[SetUp]
		public void Setup()
		{
			driver = new ChromeDriver();
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl("https://shikimori.one");

		}
		[TearDown]
		public void TearDown()
		{
			driver.Quit();
		}
		//Проверка заголовка страницы
		[Test]
		public void Is_Title_Correct()
		{
			string expected = "Шикимори - энциклопедия аниме и манги";
			Assert.AreEqual(expected, driver.Title);
		}
		//Проверка видимости объектов
		[Test]
		public void Is_Logo_Displayed()
		{
			var logo = driver.FindElement(By.CssSelector("#dashboards_show > header > div.menu-logo > a > div.logo"));
			Assert.IsTrue(logo.Displayed);
		}
		[Test]
		public void Is_News_Displayed()
		{
			var news = driver.FindElement(By.XPath("//*[@id=\"dashboards_show\"]/section/div[2]/div/div[4]/div[2]"));
			Assert.IsTrue(news.Displayed);
		}
		[Test]
		public void Is_Ongoing_List_Displayed()
		{
			var ongoingList = driver.FindElement(By.CssSelector("#dashboards_show > section > div > div > div:nth-child(1) > div.subheadline.linkheadline > a"));
			Assert.IsTrue(ongoingList.Displayed);
		}
		//Переход по ссылке
		[Test]
		public void Navigation_To_Anime_Page()
		{
			var menuToggle = wait.Until(d =>
				d.FindElement(By.CssSelector("#dashboards_show > header > div.menu-logo > div > span.submenu-triangle.icon-home")));
			menuToggle.Click();

			var animeLink = driver.FindElement(By.CssSelector("a.icon-anime[title='Аниме']"));
			((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", animeLink);

			wait.Until(d => d.Url.Contains("/animes"));
			Assert.That(driver.Url, Does.Contain("/animes"));
		}
		[Test]
		public void Navigation_To_Manga_Page()
		{
			var mangaLink = wait.Until(d =>d.FindElement(By.XPath("//*[@id=\"dashboards_show\"]/section/div[2]/div/div[3]/div/div[2]/div[1]/div[2]/div")));
			mangaLink.Click();

			
			wait.Until(d => d.Url.Contains("/mangas"));
			Assert.That(driver.Url, Does.Contain("/mangas"));
		}
		
		//Заполнение текстового поля
		[Test]
		public void Fill_Search_Field()
		{
			var search = wait.Until(d => d.FindElement(By.CssSelector("#dashboards_show > header > div.menu-icon.search.mobile")));
			((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", search);

			string searchText = "Наруто";
			var searchInput = wait.Until(d=>d.FindElement(By.XPath("//*[@id=\"dashboards_show\"]/header/div[3]/label/input")));
			searchInput.SendKeys(searchText);
			Assert.AreEqual(searchText, searchInput.GetAttribute("value"));
		}
		//Эмуляция нажатия на кнопку
		[Test]
		public void Press_Forum_Button()
		{
			var forumButton = wait.Until(d=>d.FindElement(By.XPath("//*[@id=\"dashboards_show\"]/section/div/div/div[3]/div/div[2]/div[2]/div[3]/div[2]/a")));
			forumButton.Click();

			wait.Until(d => d.Url.Contains("/forum"));
			Assert.That(driver.Url, Does.Contain("/forum"));
		}
		[Test]
		public void Search_Anime_And_Go_To_Animes_Url() 
		{
			var search = wait.Until(d => d.FindElement(By.CssSelector("#dashboards_show > header > div.menu-icon.search.mobile")));
			((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", search);

			string searchText = "Наруто";
			var searchInput = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"dashboards_show\"]/header/div[3]/label/input")));
			searchInput.SendKeys(searchText);
			searchInput.SendKeys(Keys.Enter);

			wait.Until(d=>d.Url.Contains("/animes?search"));
			var anime = wait.Until(d =>d.FindElement(By.CssSelector("a[href*='/animes/z20-naruto']")));
			anime.Click();
			wait.Until(d => d.Url.Contains("/z20-naruto"));
			Assert.That(driver.Url, Does.Contain("/z20-naruto"));
		}
		
		[Test]
		public void Add_To_List_Button_Not_Changing_If_User_Not_Authorized()
		{
			driver.Navigate().GoToUrl("https://shikimori.one/animes/z20-naruto");
			try
			{
				var signInButton = wait.Until(d => d.FindElement(By.CssSelector("header a[href*='/users/sign_in']")));
				Assert.IsTrue(signInButton.Displayed);
			}
			catch (WebDriverTimeoutException)
			{
				Assert.Fail("Нет кнопки входа");
			}

			var addButton = wait.Until(d =>	d.FindElement(By.XPath("//*[@id=\"animes_show\"]/section/div/div[2]/div/div/div[1]/div[1]/div[1]/div[2]/div/div/form/div[1]")));
			string addButtonText = addButton.Text;
			addButton.Click();

			var currentAddButton = addButton.Text;
			//Текст кнопки не должен меняться, так как неавторизированному пользователю нельзя добалять аниме в списки
			Assert.That(currentAddButton, Is.EqualTo(addButtonText));

		}

	}
}