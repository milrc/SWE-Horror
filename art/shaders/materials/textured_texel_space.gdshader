shader_type spatial;
render_mode blend_mix, depth_draw_opaque, diffuse_burley, specular_schlick_ggx, cull_back;

#include "../includes/texel_snap.gdshaderinc"

uniform sampler2D texture_albedo : source_color, filter_nearest, repeat_enable;
uniform sampler2D normal_map : hint_normal, filter_nearest;

uniform float roughness : hint_range(0.0, 1.0);

void fragment() {
	vec2 delta = screen_delta(UV, texture_albedo);
	LIGHT_VERTEX = texel_snap(VERTEX, delta);

	vec3 snapped_normal = texel_snap(NORMAL, delta);
	if (length(snapped_normal) > 1e-3)
		NORMAL = normalize(snapped_normal);
	
	vec4 albedo_tex = texture(texture_albedo, UV).rgba;
	ALBEDO = albedo_tex.rgb;
	ALPHA = albedo_tex.a;
	NORMAL_MAP = texture(normal_map, UV).xyz;
	ROUGHNESS = roughness;
    ALPHA_SCISSOR_THRESHOLD = 0.5f;
}