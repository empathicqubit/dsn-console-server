using System;
using System.Collections.Generic;
using Nancy;
using Nancy.TinyIoc;

namespace EmpathicQbt.ConsoleServer
{
    public class ServerBootstrapper : DefaultNancyBootstrapper
    {
        private SkyrimInterop skyrimInterop;
        public ServerBootstrapper(SkyrimInterop skyrimInterop)
        {
            this.skyrimInterop = skyrimInterop;
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<SkyrimInterop>(skyrimInterop);
        }
    }
}