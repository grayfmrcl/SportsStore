using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.Domain.Entities;
using System.Linq;
using SportStore.WebUI.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SportStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // Arrange
            // create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1" },
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }.AsQueryable());
            // create the controller
            var target = new AdminController(mock.Object);

            // Act
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0].Name, "P1");
            Assert.AreEqual(result[1].Name, "P2");
            Assert.AreEqual(result[2].Name, "P3");
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            // Arrange
            // create product repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1" },
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }.AsQueryable());
            //craete the controller
            var target = new AdminController(mock.Object);

            // Act
            Product p1 = (Product)target.Edit(1).ViewData.Model;
            Product p2 = (Product)target.Edit(2).ViewData.Model;
            Product p3 = (Product)target.Edit(3).ViewData.Model;

            // Assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual("P2", p2.Name);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            // Arrange
            // create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }.AsQueryable());
            // create the controller
            var target = new AdminController(mock.Object);

            // Act
            Product result = (Product)target.Edit(4).ViewData.Model;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange
            // create mock repository
            var mock = new Mock<IProductRepository>();
            // create the controller
            var target = new AdminController(mock.Object);
            // create the product
            var product = new Product { Name = "Test" };

            // Act
            // try to save the product
            ActionResult result = target.Edit(product, null);

            // Assert
            // check that the repository was called
            mock.Verify(m => m.SaveProduct(product));
            // check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_See_Invalid_Changes()
        {
            //Arrange
            // create mock repository
            var mock = new Mock<IProductRepository>();
            // create the controller
            var target = new AdminController(mock.Object);
            // create the product
            var product = new Product { Name = "Test" };
            // add an error to the odel state
            target.ModelState.AddModelError("error", "error");

            // Act
            // try to save the product
            ActionResult result = target.Edit(product, null);

            // Assert
            // check the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            // Arrange
            // create a product
            Product productToBeDeleted = new Product { ProductID = 2, Name = "Test" };
            // create mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "Test"},
                new Product { ProductID = 3, Name = "P3"}
            }.AsQueryable());
            // create the controller
            var target = new AdminController(mock.Object);

            // Act
            // delete the product
            target.Delete(productToBeDeleted.ProductID);

            // Assert
            // ensure that the repository delete method was called with the correct Product
            mock.Verify(m => m.DeleteProduct(productToBeDeleted.ProductID));
            
        }

    }
}
