// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: mediapipe/modules/objectron/calculators/tflite_tensors_to_objects_calculator.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Mediapipe {

  /// <summary>Holder for reflection information generated from mediapipe/modules/objectron/calculators/tflite_tensors_to_objects_calculator.proto</summary>
  public static partial class TfliteTensorsToObjectsCalculatorReflection {

    #region Descriptor
    /// <summary>File descriptor for mediapipe/modules/objectron/calculators/tflite_tensors_to_objects_calculator.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TfliteTensorsToObjectsCalculatorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ClJtZWRpYXBpcGUvbW9kdWxlcy9vYmplY3Ryb24vY2FsY3VsYXRvcnMvdGZs",
            "aXRlX3RlbnNvcnNfdG9fb2JqZWN0c19jYWxjdWxhdG9yLnByb3RvEgltZWRp",
            "YXBpcGUaJG1lZGlhcGlwZS9mcmFtZXdvcmsvY2FsY3VsYXRvci5wcm90bxpD",
            "bWVkaWFwaXBlL21vZHVsZXMvb2JqZWN0cm9uL2NhbGN1bGF0b3JzL2JlbGll",
            "Zl9kZWNvZGVyX2NvbmZpZy5wcm90byKjAwonVGZMaXRlVGVuc29yc1RvT2Jq",
            "ZWN0c0NhbGN1bGF0b3JPcHRpb25zEhMKC251bV9jbGFzc2VzGAEgASgFEhUK",
            "DW51bV9rZXlwb2ludHMYAiABKAUSIgoXbnVtX3ZhbHVlc19wZXJfa2V5cG9p",
            "bnQYAyABKAU6ATISNgoOZGVjb2Rlcl9jb25maWcYBCABKAsyHi5tZWRpYXBp",
            "cGUuQmVsaWVmRGVjb2RlckNvbmZpZxIdChJub3JtYWxpemVkX2ZvY2FsX3gY",
            "BSABKAI6ATESHQoSbm9ybWFsaXplZF9mb2NhbF95GAYgASgCOgExEicKHG5v",
            "cm1hbGl6ZWRfcHJpbmNpcGFsX3BvaW50X3gYByABKAI6ATASJwocbm9ybWFs",
            "aXplZF9wcmluY2lwYWxfcG9pbnRfeRgIIAEoAjoBMDJgCgNleHQSHC5tZWRp",
            "YXBpcGUuQ2FsY3VsYXRvck9wdGlvbnMYvv/cfSABKAsyMi5tZWRpYXBpcGUu",
            "VGZMaXRlVGVuc29yc1RvT2JqZWN0c0NhbGN1bGF0b3JPcHRpb25z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Mediapipe.CalculatorReflection.Descriptor, global::Mediapipe.BeliefDecoderConfigReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions), global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions.Parser, new[]{ "NumClasses", "NumKeypoints", "NumValuesPerKeypoint", "DecoderConfig", "NormalizedFocalX", "NormalizedFocalY", "NormalizedPrincipalPointX", "NormalizedPrincipalPointY" }, null, null, new pb::Extension[] { global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions.Extensions.Ext }, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TfLiteTensorsToObjectsCalculatorOptions : pb::IMessage<TfLiteTensorsToObjectsCalculatorOptions>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<TfLiteTensorsToObjectsCalculatorOptions> _parser = new pb::MessageParser<TfLiteTensorsToObjectsCalculatorOptions>(() => new TfLiteTensorsToObjectsCalculatorOptions());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<TfLiteTensorsToObjectsCalculatorOptions> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Mediapipe.TfliteTensorsToObjectsCalculatorReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TfLiteTensorsToObjectsCalculatorOptions() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TfLiteTensorsToObjectsCalculatorOptions(TfLiteTensorsToObjectsCalculatorOptions other) : this() {
      _hasBits0 = other._hasBits0;
      numClasses_ = other.numClasses_;
      numKeypoints_ = other.numKeypoints_;
      numValuesPerKeypoint_ = other.numValuesPerKeypoint_;
      decoderConfig_ = other.decoderConfig_ != null ? other.decoderConfig_.Clone() : null;
      normalizedFocalX_ = other.normalizedFocalX_;
      normalizedFocalY_ = other.normalizedFocalY_;
      normalizedPrincipalPointX_ = other.normalizedPrincipalPointX_;
      normalizedPrincipalPointY_ = other.normalizedPrincipalPointY_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TfLiteTensorsToObjectsCalculatorOptions Clone() {
      return new TfLiteTensorsToObjectsCalculatorOptions(this);
    }

    /// <summary>Field number for the "num_classes" field.</summary>
    public const int NumClassesFieldNumber = 1;
    private readonly static int NumClassesDefaultValue = 0;

    private int numClasses_;
    /// <summary>
    /// The number of output classes predicted by the detection model.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumClasses {
      get { if ((_hasBits0 & 1) != 0) { return numClasses_; } else { return NumClassesDefaultValue; } }
      set {
        _hasBits0 |= 1;
        numClasses_ = value;
      }
    }
    /// <summary>Gets whether the "num_classes" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumClasses {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "num_classes" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumClasses() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "num_keypoints" field.</summary>
    public const int NumKeypointsFieldNumber = 2;
    private readonly static int NumKeypointsDefaultValue = 0;

    private int numKeypoints_;
    /// <summary>
    /// The number of predicted keypoints.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumKeypoints {
      get { if ((_hasBits0 & 2) != 0) { return numKeypoints_; } else { return NumKeypointsDefaultValue; } }
      set {
        _hasBits0 |= 2;
        numKeypoints_ = value;
      }
    }
    /// <summary>Gets whether the "num_keypoints" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumKeypoints {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "num_keypoints" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumKeypoints() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "num_values_per_keypoint" field.</summary>
    public const int NumValuesPerKeypointFieldNumber = 3;
    private readonly static int NumValuesPerKeypointDefaultValue = 2;

    private int numValuesPerKeypoint_;
    /// <summary>
    /// The dimension of each keypoint, e.g. number of values predicted for each
    /// keypoint.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumValuesPerKeypoint {
      get { if ((_hasBits0 & 4) != 0) { return numValuesPerKeypoint_; } else { return NumValuesPerKeypointDefaultValue; } }
      set {
        _hasBits0 |= 4;
        numValuesPerKeypoint_ = value;
      }
    }
    /// <summary>Gets whether the "num_values_per_keypoint" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumValuesPerKeypoint {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "num_values_per_keypoint" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumValuesPerKeypoint() {
      _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "decoder_config" field.</summary>
    public const int DecoderConfigFieldNumber = 4;
    private global::Mediapipe.BeliefDecoderConfig decoderConfig_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Mediapipe.BeliefDecoderConfig DecoderConfig {
      get { return decoderConfig_; }
      set {
        decoderConfig_ = value;
      }
    }

    /// <summary>Field number for the "normalized_focal_x" field.</summary>
    public const int NormalizedFocalXFieldNumber = 5;
    private readonly static float NormalizedFocalXDefaultValue = 1F;

    private float normalizedFocalX_;
    /// <summary>
    /// Camera focal length along x, normalized by width/2.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float NormalizedFocalX {
      get { if ((_hasBits0 & 8) != 0) { return normalizedFocalX_; } else { return NormalizedFocalXDefaultValue; } }
      set {
        _hasBits0 |= 8;
        normalizedFocalX_ = value;
      }
    }
    /// <summary>Gets whether the "normalized_focal_x" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNormalizedFocalX {
      get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "normalized_focal_x" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNormalizedFocalX() {
      _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "normalized_focal_y" field.</summary>
    public const int NormalizedFocalYFieldNumber = 6;
    private readonly static float NormalizedFocalYDefaultValue = 1F;

    private float normalizedFocalY_;
    /// <summary>
    /// Camera focal length along y, normalized by height/2.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float NormalizedFocalY {
      get { if ((_hasBits0 & 16) != 0) { return normalizedFocalY_; } else { return NormalizedFocalYDefaultValue; } }
      set {
        _hasBits0 |= 16;
        normalizedFocalY_ = value;
      }
    }
    /// <summary>Gets whether the "normalized_focal_y" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNormalizedFocalY {
      get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "normalized_focal_y" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNormalizedFocalY() {
      _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "normalized_principal_point_x" field.</summary>
    public const int NormalizedPrincipalPointXFieldNumber = 7;
    private readonly static float NormalizedPrincipalPointXDefaultValue = 0F;

    private float normalizedPrincipalPointX_;
    /// <summary>
    /// Camera principle point x, normalized by width/2, origin is image center.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float NormalizedPrincipalPointX {
      get { if ((_hasBits0 & 32) != 0) { return normalizedPrincipalPointX_; } else { return NormalizedPrincipalPointXDefaultValue; } }
      set {
        _hasBits0 |= 32;
        normalizedPrincipalPointX_ = value;
      }
    }
    /// <summary>Gets whether the "normalized_principal_point_x" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNormalizedPrincipalPointX {
      get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "normalized_principal_point_x" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNormalizedPrincipalPointX() {
      _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "normalized_principal_point_y" field.</summary>
    public const int NormalizedPrincipalPointYFieldNumber = 8;
    private readonly static float NormalizedPrincipalPointYDefaultValue = 0F;

    private float normalizedPrincipalPointY_;
    /// <summary>
    /// Camera principle point y, normalized by height/2, origin is image center.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float NormalizedPrincipalPointY {
      get { if ((_hasBits0 & 64) != 0) { return normalizedPrincipalPointY_; } else { return NormalizedPrincipalPointYDefaultValue; } }
      set {
        _hasBits0 |= 64;
        normalizedPrincipalPointY_ = value;
      }
    }
    /// <summary>Gets whether the "normalized_principal_point_y" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNormalizedPrincipalPointY {
      get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "normalized_principal_point_y" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNormalizedPrincipalPointY() {
      _hasBits0 &= ~64;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as TfLiteTensorsToObjectsCalculatorOptions);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(TfLiteTensorsToObjectsCalculatorOptions other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (NumClasses != other.NumClasses) return false;
      if (NumKeypoints != other.NumKeypoints) return false;
      if (NumValuesPerKeypoint != other.NumValuesPerKeypoint) return false;
      if (!object.Equals(DecoderConfig, other.DecoderConfig)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(NormalizedFocalX, other.NormalizedFocalX)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(NormalizedFocalY, other.NormalizedFocalY)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(NormalizedPrincipalPointX, other.NormalizedPrincipalPointX)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(NormalizedPrincipalPointY, other.NormalizedPrincipalPointY)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (HasNumClasses) hash ^= NumClasses.GetHashCode();
      if (HasNumKeypoints) hash ^= NumKeypoints.GetHashCode();
      if (HasNumValuesPerKeypoint) hash ^= NumValuesPerKeypoint.GetHashCode();
      if (decoderConfig_ != null) hash ^= DecoderConfig.GetHashCode();
      if (HasNormalizedFocalX) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(NormalizedFocalX);
      if (HasNormalizedFocalY) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(NormalizedFocalY);
      if (HasNormalizedPrincipalPointX) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(NormalizedPrincipalPointX);
      if (HasNormalizedPrincipalPointY) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(NormalizedPrincipalPointY);
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (HasNumClasses) {
        output.WriteRawTag(8);
        output.WriteInt32(NumClasses);
      }
      if (HasNumKeypoints) {
        output.WriteRawTag(16);
        output.WriteInt32(NumKeypoints);
      }
      if (HasNumValuesPerKeypoint) {
        output.WriteRawTag(24);
        output.WriteInt32(NumValuesPerKeypoint);
      }
      if (decoderConfig_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(DecoderConfig);
      }
      if (HasNormalizedFocalX) {
        output.WriteRawTag(45);
        output.WriteFloat(NormalizedFocalX);
      }
      if (HasNormalizedFocalY) {
        output.WriteRawTag(53);
        output.WriteFloat(NormalizedFocalY);
      }
      if (HasNormalizedPrincipalPointX) {
        output.WriteRawTag(61);
        output.WriteFloat(NormalizedPrincipalPointX);
      }
      if (HasNormalizedPrincipalPointY) {
        output.WriteRawTag(69);
        output.WriteFloat(NormalizedPrincipalPointY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasNumClasses) {
        output.WriteRawTag(8);
        output.WriteInt32(NumClasses);
      }
      if (HasNumKeypoints) {
        output.WriteRawTag(16);
        output.WriteInt32(NumKeypoints);
      }
      if (HasNumValuesPerKeypoint) {
        output.WriteRawTag(24);
        output.WriteInt32(NumValuesPerKeypoint);
      }
      if (decoderConfig_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(DecoderConfig);
      }
      if (HasNormalizedFocalX) {
        output.WriteRawTag(45);
        output.WriteFloat(NormalizedFocalX);
      }
      if (HasNormalizedFocalY) {
        output.WriteRawTag(53);
        output.WriteFloat(NormalizedFocalY);
      }
      if (HasNormalizedPrincipalPointX) {
        output.WriteRawTag(61);
        output.WriteFloat(NormalizedPrincipalPointX);
      }
      if (HasNormalizedPrincipalPointY) {
        output.WriteRawTag(69);
        output.WriteFloat(NormalizedPrincipalPointY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (HasNumClasses) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(NumClasses);
      }
      if (HasNumKeypoints) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(NumKeypoints);
      }
      if (HasNumValuesPerKeypoint) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(NumValuesPerKeypoint);
      }
      if (decoderConfig_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(DecoderConfig);
      }
      if (HasNormalizedFocalX) {
        size += 1 + 4;
      }
      if (HasNormalizedFocalY) {
        size += 1 + 4;
      }
      if (HasNormalizedPrincipalPointX) {
        size += 1 + 4;
      }
      if (HasNormalizedPrincipalPointY) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(TfLiteTensorsToObjectsCalculatorOptions other) {
      if (other == null) {
        return;
      }
      if (other.HasNumClasses) {
        NumClasses = other.NumClasses;
      }
      if (other.HasNumKeypoints) {
        NumKeypoints = other.NumKeypoints;
      }
      if (other.HasNumValuesPerKeypoint) {
        NumValuesPerKeypoint = other.NumValuesPerKeypoint;
      }
      if (other.decoderConfig_ != null) {
        if (decoderConfig_ == null) {
          DecoderConfig = new global::Mediapipe.BeliefDecoderConfig();
        }
        DecoderConfig.MergeFrom(other.DecoderConfig);
      }
      if (other.HasNormalizedFocalX) {
        NormalizedFocalX = other.NormalizedFocalX;
      }
      if (other.HasNormalizedFocalY) {
        NormalizedFocalY = other.NormalizedFocalY;
      }
      if (other.HasNormalizedPrincipalPointX) {
        NormalizedPrincipalPointX = other.NormalizedPrincipalPointX;
      }
      if (other.HasNormalizedPrincipalPointY) {
        NormalizedPrincipalPointY = other.NormalizedPrincipalPointY;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            NumClasses = input.ReadInt32();
            break;
          }
          case 16: {
            NumKeypoints = input.ReadInt32();
            break;
          }
          case 24: {
            NumValuesPerKeypoint = input.ReadInt32();
            break;
          }
          case 34: {
            if (decoderConfig_ == null) {
              DecoderConfig = new global::Mediapipe.BeliefDecoderConfig();
            }
            input.ReadMessage(DecoderConfig);
            break;
          }
          case 45: {
            NormalizedFocalX = input.ReadFloat();
            break;
          }
          case 53: {
            NormalizedFocalY = input.ReadFloat();
            break;
          }
          case 61: {
            NormalizedPrincipalPointX = input.ReadFloat();
            break;
          }
          case 69: {
            NormalizedPrincipalPointY = input.ReadFloat();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            NumClasses = input.ReadInt32();
            break;
          }
          case 16: {
            NumKeypoints = input.ReadInt32();
            break;
          }
          case 24: {
            NumValuesPerKeypoint = input.ReadInt32();
            break;
          }
          case 34: {
            if (decoderConfig_ == null) {
              DecoderConfig = new global::Mediapipe.BeliefDecoderConfig();
            }
            input.ReadMessage(DecoderConfig);
            break;
          }
          case 45: {
            NormalizedFocalX = input.ReadFloat();
            break;
          }
          case 53: {
            NormalizedFocalY = input.ReadFloat();
            break;
          }
          case 61: {
            NormalizedPrincipalPointX = input.ReadFloat();
            break;
          }
          case 69: {
            NormalizedPrincipalPointY = input.ReadFloat();
            break;
          }
        }
      }
    }
    #endif

    #region Extensions
    /// <summary>Container for extensions for other messages declared in the TfLiteTensorsToObjectsCalculatorOptions message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Extensions {
      public static readonly pb::Extension<global::Mediapipe.CalculatorOptions, global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions> Ext =
        new pb::Extension<global::Mediapipe.CalculatorOptions, global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions>(263667646, pb::FieldCodec.ForMessage(2109341170, global::Mediapipe.TfLiteTensorsToObjectsCalculatorOptions.Parser));
    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code