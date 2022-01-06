using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Bunit;
using Bunit.TestDoubles;
using RichardSzalay.MockHttp;
using Xunit;

namespace BlazingPizza.Tests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.egilhansen.com/docs/getting-started/
    /// </summary>
    public class MockExample : TestContext
    {
        [Fact]
        public void ChooseAPizza()
        {
            //Arrange
            using var ctx = new TestContext();
            var mock = ctx.Services.AddMockHttpClient();
            mock.When("/specials").RespondJson(new List<PizzaSpecial> {new PizzaSpecial()});

            //Act
            var cut = ctx.RenderComponent<Client.Pages.Index>();

            //Assert that content of the paragraph shows choose a pizza.
            Assert.Contains("Choose a pizza", cut.Markup);
        }

        [Fact]
        public void DeleteOrder()
        {
            //Arrange Pizza
            using var ctx = new TestContext();

            var mock = ctx.Services.AddMockHttpClient();
            var list = new List<PizzaSpecial> { new PizzaSpecial() { Name = "Mr.Bean" }, new PizzaSpecial() { Name = "Mr.Bean" }, new PizzaSpecial() { Name = "Mr.Bean" } };
            mock.When("/specials").RespondJson(list);
            mock.When("/toppings").RespondJson(new List<Topping> { new Topping() { Name = "Mr.Bean", Price = 50 } });
            //Act

            var cut = ctx.RenderComponent<Client.Pages.Index>();
            Assert.NotEqual(0, cut.RenderCount);
            Thread.Sleep(50);
            cut.WaitForAssertion(() => cut.Find("li"));
            cut.Find("li").Click();

            cut.WaitForAssertion(() => cut.Find("button"));


            cut.FindAll("button")[2].Click();

            Assert.Contains("Your order", cut.Markup);

            cut.WaitForAssertion(() => cut.Find("a"));
            cut.Find("a").Click();
            Thread.Sleep(500);
            //Assert
            Assert.DoesNotContain("Your order", cut.Markup);

        }
        [Fact]
        public void CustomizedPizzaGivesCorrectResult()
        {
            //Arrange Pizza
            using var ctx = new TestContext();

            var mock = ctx.Services.AddMockHttpClient();
            var list = new List<PizzaSpecial> { new PizzaSpecial() { Name = "Mr.Bean" }, new PizzaSpecial() { Name = "Mr.Bean" }, new PizzaSpecial() { Name = "Mr.Bean" } };
            mock.When("/specials").RespondJson(list);
            mock.When("/toppings").RespondJson(new List<Topping>() { new Topping() { Name = "Pepperoni", Price = 50 },
                                                                   new Topping() { Name = "Kebab", Price = 100},
                                                                   new Topping() { Name = "Skinke", Price = 20 },
                                                                   new Topping() { Name = "Salat", Price = 8 },
                                                                   new Topping() { Name = "Ost" , Price = 10 },
                                                                   new Topping() { Name = "Østers", Price = 20 },
                                                                   new Topping() { Name = "Danmark", Price = 1000000 } });
            //Act

            var cut = ctx.RenderComponent<Client.Pages.Index>();
            Thread.Sleep(50);
            cut.WaitForAssertion(() => cut.Find("li"));
            cut.Find("li").Click();


            for (int i = 0; i < 6; i++)
            {
                cut.WaitForAssertion(() => cut.Find("select"));
                cut.Find("select").Change(i.ToString());
                
            }
            Thread.Sleep(50);
            Assert.Contains("<div>(maximum reached)</div>", cut.Markup);
            Assert.Contains("208", cut.Markup);

        }
    }
}
