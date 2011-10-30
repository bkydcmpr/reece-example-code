using System;

namespace Reece.Example.MVC.UnitTesting.Biz.Interfaces
{
    public interface IProspectRequest
    {
        Guid AddProspect(string name, string phone);
    }
}
