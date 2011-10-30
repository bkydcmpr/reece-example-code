using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Reece.Example.MVC.UnitTesting.Biz.Interfaces;
using Reece.Example.MVC.UnitTesting.Controllers;
using Reece.Example.MVC.UnitTesting.Models;

namespace Reece.Example.MVC.UnitTesting.Tests
{
    [TestFixture]
    public class ProspectController_Tests
    {
        private ProspectController target;
        private Mock<IProspectRequest> _mock;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<IProspectRequest>();
            target = new ProspectController {ProspectRequest = _mock.Object};
        }

        private Prospect GetNewProspect()
        {
            return new Prospect()
                       {
                           Name = "NameTest",
                           Phone = "PhoneTest"
                       };
        }

        [Test]
        public void New_ReturnsView_Test()
        {
            var result = target.New() as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void New_PassesProspectModelToView_Test()
        {
            var view = target.New() as ViewResult;
            var result = view.Model as Prospect;
            Assert.IsNotNull(result);
        }

        [Test]
        public void New_PassesEmptyGuidToView_Test()
        {
            var view = target.New() as ViewResult;
            var result = view.Model as Prospect;
            Assert.AreEqual(Guid.Empty, result.ID);
        }

        [Test]
        public void New_PassesEmptyNameToView_Test()
        {
            var view = target.New() as ViewResult;
            var result = view.Model as Prospect;
            Assert.IsNullOrEmpty(result.Name);
        }

        [Test]
        public void New_PassesEmptyPhoneToView_Test()
        {
            var view = target.New() as ViewResult;
            var result = view.Model as Prospect;
            Assert.IsNullOrEmpty(result.Phone);
        }

        [Test]
        public void New_post_PassesNameToBiz_Test()
        {
            var expected = GetNewProspect();
            var calledCount = 0;
            _mock.Setup(x => x.AddProspect(It.Is<string>((e) => e == expected.Name), It.IsAny<string>()))
                .Returns(Guid.NewGuid)
                .Callback(() => ++calledCount);
            target.New(expected);
            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void New_post_PassesPhoneToBiz_Test()
        {
            var expected = GetNewProspect();
            var calledCount = 0;
            _mock.Setup(x => x.AddProspect(It.IsAny<string>(), It.Is<string>((e) => e == expected.Phone)))
                .Returns(Guid.NewGuid)
                .Callback(() => ++calledCount);
            target.New(expected);
            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void New_post_ReturnsJsonResult_Test()
        {
            var result = target.New(GetNewProspect()) as JsonResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void New_post_ReturnsProspectAsData_Test()
        {
            _mock.Setup(x => x.AddProspect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Guid.NewGuid);
            target.New(GetNewProspect());
            var json = target.New(GetNewProspect()) as JsonResult;
            var result = json.Data as Prospect;
            Assert.IsNotNull(result);
        }

        [Test]
        public void New_post_ReturnsPhone_Test()
        {
            var expected = GetNewProspect();
            _mock.Setup(x => x.AddProspect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Guid.NewGuid);
            target.New(expected);
            var json = target.New(GetNewProspect()) as JsonResult;
            var result = json.Data as Prospect;
            Assert.AreEqual(expected.Phone, result.Phone);
        }

        [Test]
        public void New_post_ReturnsName_Test()
        {
            var expected = GetNewProspect();
            _mock.Setup(x => x.AddProspect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Guid.NewGuid);
            target.New(expected);
            var json = target.New(GetNewProspect()) as JsonResult;
            var result = json.Data as Prospect;
            Assert.AreEqual(expected.Name, result.Name);
        }

        [Test]
        public void New_post_ReturnsID_Test()
        {
            var expected = Guid.NewGuid();
            _mock.Setup(x => x.AddProspect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(expected);
            target.New(GetNewProspect());
            var json = target.New(GetNewProspect()) as JsonResult;
            var result = json.Data as Prospect;
            Assert.AreEqual(expected, result.ID);
        }
    }
}
