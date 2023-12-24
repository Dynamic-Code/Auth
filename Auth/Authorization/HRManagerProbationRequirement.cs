using Microsoft.AspNetCore.Authorization;

// purpose - To create a auth handler which calculate the probation period of the emp
namespace Auth.Authorization
{
    public class HRManagerProbationRequirement: IAuthorizationRequirement // implements IAuthorizationRequirement
                                                                          // because AuthorizationHandler need a class of type 
                                                                          // IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }
    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
           if( !context.User.HasClaim(x =>x.Type == "EmploymentDate")) // if the user identity has not Employementdate claim 
               return Task.CompletedTask;

            var empDate = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmploymentDate").Value);
            var peroid = DateTime.Now - empDate;
            if (peroid.Days > 30 * requirement.ProbationMonths)
                context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }
}
