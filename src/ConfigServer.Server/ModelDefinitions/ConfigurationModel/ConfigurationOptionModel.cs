using ConfigServer.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal abstract class ConfigurationOptionModel : ConfigurationModel
    {
        public ConfigurationOptionModel(string name, Type type, Type configurationSetType) : base(name, type, configurationSetType, true)
        {
        }

        public abstract IOptionSet BuildOptionSet(IEnumerable souce);
        public abstract string GetKeyFromObject(object value);
        public Type StoredType { get; protected set; }
        public abstract object NewItemInstance();
    }

    internal class ConfigurationOptionModel<TOption, TConfigurationSet> : ConfigurationOptionModel where TConfigurationSet : ConfigurationSet where TOption : class, new()
    {
        private readonly Func<TOption, string> keySelector;
        private readonly Func<TOption, object> descriptionSelector;
        private readonly Func<TConfigurationSet, OptionSet<TOption>> optionSelector;
        private readonly Action<TConfigurationSet, OptionSet<TOption>> configSetter;

        public ConfigurationOptionModel(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<TConfigurationSet, OptionSet<TOption>> optionSelector, Action<TConfigurationSet, OptionSet<TOption>> configSetter) : base(name, typeof(TOption), typeof(TConfigurationSet))
        {
            this.keySelector = keySelector;
            this.descriptionSelector = descriptionSelector;
            this.optionSelector = optionSelector;
            this.configSetter = configSetter;
            StoredType = typeof(List<TOption>);
        }

        private string DescriptionSelector(TOption option)
        {
            return descriptionSelector(option).ToString();
        }

        public override IOptionSet BuildOptionSet(IEnumerable source)
        {
            return new OptionSet<TOption>(source, keySelector, DescriptionSelector);
        }

        public override object GetConfigurationFromConfigurationSet(object configurationSet)
        {
            return optionSelector((TConfigurationSet)configurationSet);
        }

        public override ConfigInstance GetConfigInstanceFromConfigurationSet(object configurationSet)
        {
            var castConfigurationSet = (TConfigurationSet)configurationSet;
            var config = optionSelector(castConfigurationSet);
            return new ConfigCollectionInstance<TOption>(config, castConfigurationSet.Instance);
        }

        public override void SetConfigurationOnConfigurationSet(object configurationSet, object value)
        {
            configSetter((TConfigurationSet)configurationSet, (OptionSet<TOption>)value);
        }

        public override string GetKeyFromObject(object value) => keySelector((TOption)value);

        public override object NewItemInstance() => new TOption();
    }
}
