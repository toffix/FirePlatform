﻿using FirePlatform.Models.Containers;
using FirePlatform.Models.Models;
using FirePlatform.Repositories;
using FirePlatform.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FirePlatform.Services.Services
{
    public class FormService : BaseService<FormService, FormRepository, Form>
    {
        public FormService
            (
                BaseRepository<Form, FormRepository> baseRepository,
                Repository repository
            ) : base(baseRepository, repository)
        {

        }

        public async Task<ServiceContainer<UserForm>> AddUserToForm(int IdForm, int IdUser)
        {
            var UserForm = new UserForm
            {
                FormId = IdForm,
                UserId = IdUser
            };

            var container = new ServiceContainer<UserForm>();
            container.DataObject = await Repository.GetUserFormRepository().Create(UserForm);

            return container;
        }

        public async Task<ServiceContainer<Form>> GetFormsByUserId(int IdUser)
        {
            var forms = Repository.GetFormRepository().GetIQueryable(x => x.UserForms.Any(y => y.UserId == IdUser));

            var container = new ServiceContainer<Form>();
            container.DataCollection = await forms.ToListAsync();

            return container;
        }

        public async Task<ServiceContainer<Form>> GetAllForms()
        {
            var forms = Repository.GetFormRepository().GetIQueryable();

            var container = new ServiceContainer<Form>();
            container.DataCollection = await forms.ToListAsync();

            return container;
        }
    }
}
