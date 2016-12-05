﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GetBanks.BankService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfString", Namespace="http://tempuri.org/", ItemName="string")]
    [System.SerializableAttribute()]
    public class ArrayOfString : System.Collections.Generic.List<string> {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BankService.RuleBaseSoap")]
    public interface RuleBaseSoap {
        
        // CODEGEN: Generating message contract since element name GetBankQueuesResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetBankQueues", ReplyAction="*")]
        GetBanks.BankService.GetBankQueuesResponse GetBankQueues(GetBanks.BankService.GetBankQueuesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetBankQueues", ReplyAction="*")]
        System.Threading.Tasks.Task<GetBanks.BankService.GetBankQueuesResponse> GetBankQueuesAsync(GetBanks.BankService.GetBankQueuesRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetBankQueuesRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetBankQueues", Namespace="http://tempuri.org/", Order=0)]
        public GetBanks.BankService.GetBankQueuesRequestBody Body;
        
        public GetBankQueuesRequest() {
        }
        
        public GetBankQueuesRequest(GetBanks.BankService.GetBankQueuesRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetBankQueuesRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int Amount;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int CreditScore;
        
        public GetBankQueuesRequestBody() {
        }
        
        public GetBankQueuesRequestBody(int Amount, int CreditScore) {
            this.Amount = Amount;
            this.CreditScore = CreditScore;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetBankQueuesResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetBankQueuesResponse", Namespace="http://tempuri.org/", Order=0)]
        public GetBanks.BankService.GetBankQueuesResponseBody Body;
        
        public GetBankQueuesResponse() {
        }
        
        public GetBankQueuesResponse(GetBanks.BankService.GetBankQueuesResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetBankQueuesResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public GetBanks.BankService.ArrayOfString GetBankQueuesResult;
        
        public GetBankQueuesResponseBody() {
        }
        
        public GetBankQueuesResponseBody(GetBanks.BankService.ArrayOfString GetBankQueuesResult) {
            this.GetBankQueuesResult = GetBankQueuesResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface RuleBaseSoapChannel : GetBanks.BankService.RuleBaseSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RuleBaseSoapClient : System.ServiceModel.ClientBase<GetBanks.BankService.RuleBaseSoap>, GetBanks.BankService.RuleBaseSoap {
        
        public RuleBaseSoapClient() {
        }
        
        public RuleBaseSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RuleBaseSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RuleBaseSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RuleBaseSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetBanks.BankService.GetBankQueuesResponse GetBanks.BankService.RuleBaseSoap.GetBankQueues(GetBanks.BankService.GetBankQueuesRequest request) {
            return base.Channel.GetBankQueues(request);
        }
        
        public GetBanks.BankService.ArrayOfString GetBankQueues(int Amount, int CreditScore) {
            GetBanks.BankService.GetBankQueuesRequest inValue = new GetBanks.BankService.GetBankQueuesRequest();
            inValue.Body = new GetBanks.BankService.GetBankQueuesRequestBody();
            inValue.Body.Amount = Amount;
            inValue.Body.CreditScore = CreditScore;
            GetBanks.BankService.GetBankQueuesResponse retVal = ((GetBanks.BankService.RuleBaseSoap)(this)).GetBankQueues(inValue);
            return retVal.Body.GetBankQueuesResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<GetBanks.BankService.GetBankQueuesResponse> GetBanks.BankService.RuleBaseSoap.GetBankQueuesAsync(GetBanks.BankService.GetBankQueuesRequest request) {
            return base.Channel.GetBankQueuesAsync(request);
        }
        
        public System.Threading.Tasks.Task<GetBanks.BankService.GetBankQueuesResponse> GetBankQueuesAsync(int Amount, int CreditScore) {
            GetBanks.BankService.GetBankQueuesRequest inValue = new GetBanks.BankService.GetBankQueuesRequest();
            inValue.Body = new GetBanks.BankService.GetBankQueuesRequestBody();
            inValue.Body.Amount = Amount;
            inValue.Body.CreditScore = CreditScore;
            return ((GetBanks.BankService.RuleBaseSoap)(this)).GetBankQueuesAsync(inValue);
        }
    }
}
