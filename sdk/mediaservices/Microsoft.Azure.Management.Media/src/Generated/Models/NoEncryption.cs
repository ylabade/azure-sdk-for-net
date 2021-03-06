// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.Media.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Class for NoEncryption scheme
    /// </summary>
    public partial class NoEncryption
    {
        /// <summary>
        /// Initializes a new instance of the NoEncryption class.
        /// </summary>
        public NoEncryption()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the NoEncryption class.
        /// </summary>
        /// <param name="enabledProtocols">Representing supported
        /// protocols</param>
        public NoEncryption(EnabledProtocols enabledProtocols = default(EnabledProtocols))
        {
            EnabledProtocols = enabledProtocols;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets representing supported protocols
        /// </summary>
        [JsonProperty(PropertyName = "enabledProtocols")]
        public EnabledProtocols EnabledProtocols { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (EnabledProtocols != null)
            {
                EnabledProtocols.Validate();
            }
        }
    }
}
