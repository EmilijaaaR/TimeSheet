using Application.ViewEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorUserResponse
{
    internal class CreateUserResponse : IRequest<CreateUserResponse>
    {
        public UserView User { get; set; }
        public bool Success { get; set; }
    }
}
