//base include for outline
#ifndef MK_TOON_OUTLINE_ONLY_BASE
	#define MK_TOON_OUTLINE_ONLY_BASE
	/////////////////////////////////////////////////////////////////////////////////////////////
	// VERTEX SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	VertexOutputOutlineOnly outlinevert(VertexInputOutlineOnly v)
	{
		float3 worldScale = float3(
			length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
			length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
			length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
			);
		UNITY_SETUP_INSTANCE_ID(v);
		VertexOutputOutlineOnly o;
		UNITY_INITIALIZE_OUTPUT(VertexOutputOutlineOnly, o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		v.vertex.xyz += normalize(v.normal)/worldScale*_OutlineSize;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.color = _OutlineColor;

		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////
	// FRAGMENT SHADER
	/////////////////////////////////////////////////////////////////////////////////////////////
	fixed4 outlinefrag(VertexOutputOutlineOnly o) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(o);
		UNITY_APPLY_FOG(o.fogCoord, o.color);
		return o.color;
	}
#endif