using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {           

            UserRequest u = new UserRequest {Name="a",SurName="b"  };
            RoleRequest r = new RoleRequest { Role1 = "r1", Role2 = "r2" };

            var m = new Mediator();

            var uResp=m.ToDo<UserRequest,UserResponse>(u);
            var rResp = m.ToDo<RoleRequest, RoleResponse>(r);

            Console.WriteLine(uResp.FullName);
            Console.WriteLine(rResp.Roles);
        }
    }

    public interface IRequest
    {
    }
    public interface IResponse
    {
    }

    public class UserRequest:IRequest
    {
        public string Name { get; set; }
        public string SurName { get; set; }
    }
    public class UserResponse : IResponse
    {
        public string FullName { get; set; }       
    }

    public class RoleRequest:IRequest
    {
        public string Role1 { get; set; }
        public string Role2 { get; set; }
    }
    public class RoleResponse : IResponse
    {
        public string Roles { get; set; }        
    }

    public interface IRequestHandler<in X,out Y> where X:IRequest where Y:IResponse
    {
        Y Handler(X x);
    }

    public class UserHandler : IRequestHandler<UserRequest, UserResponse>
    {
        public UserResponse Handler(UserRequest x)
        {
            return new UserResponse { FullName = x.Name + " " + x.SurName };
        }
    }

    public class ProductHandler : IRequestHandler<RoleRequest, RoleResponse>
    {
        public RoleResponse Handler(RoleRequest x)
        {
             return new RoleResponse { Roles = x.Role1 + " " + x.Role2 };
        }
    }

    public class Mediator
    {
        public Resp ToDo<Req,Resp>(Req req)where Req:IRequest where Resp :IResponse
        {
            var t = typeof(IRequestHandler<,>).MakeGenericType(typeof(Req), typeof(Resp));

            var currentType=AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => t.IsAssignableFrom(x) && !x.IsInterface&&!x.IsAbstract).FirstOrDefault();
                
            var ins=Activator.CreateInstance(currentType);
            return ((IRequestHandler<Req, Resp>)ins).Handler(req);
        }
    }
}
