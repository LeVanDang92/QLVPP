using OSM.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Application.Features.BaseSetup.UserSetup.DeleteUser
{
    public sealed record DeleteUserCommand(string UserName) : ICommand;
}
