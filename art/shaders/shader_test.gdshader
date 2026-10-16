shader_type spatial;
render_mode specular_toon;

uniform sampler2D override_texture : source_color, filter_nearest;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 new_color = texture(override_texture, UV);
	
	ALBEDO = new_color.rgb;
}

//void light() {
	////SPECULAR_LIGHT += clamp(dot(NORMAL, LIGHT), 0.0, 1.0) * ATTENUATION * LIGHT_COLOR / PI;
	//
	////SPECULAR_LIGHT = vec3(0.0, 0.0, 0.0);
//}
