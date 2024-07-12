using Application.ViewEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserRequest
{
    public class GetUserRequest : IRequest<UserView>
    {
        public int Id { get; set; }
    }
}
