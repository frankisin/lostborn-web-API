using System;
namespace lostborn_web_API.Utilities
{
	public class JwtConfig
	{
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        // Add other configuration properties as needed

        public JwtConfig()
		{

		}
	}
}

