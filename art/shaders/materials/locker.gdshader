shader_type spatial;
render_mode blend_mix, depth_draw_opaque, diffuse_burley, specular_schlick_ggx, cull_back;

#include "../includes/texel_snap.gdshaderinc"

uniform sampler2D texture_albedo : source_color, filter_nearest, repeat_enable;
uniform sampler2D normal_map : hint_normal, filter_nearest;

uniform float roughness : hint_range(0.0, 1.0) = 0.7;
uniform float max_displacement : hint_range(0.0, 1.0) = 0.05;
uniform float max_distance : hint_range(0, 50.0) = 5.0;
uniform float effect_speed = 1.0;
uniform float distance_period = 1.0;
uniform float open_locker_pos = 0.0;
uniform float open_locker_influence = 1.0;
uniform sampler2D distance_to_displacement : repeat_disable;

void vertex() {
	//calculate power of effect
	float dist = length((MODELVIEW_MATRIX * vec4(VERTEX, 1.0)).xyz); //get distance from camera
	float power = texture(distance_to_displacement, vec2(dist/max_distance, 0.0)).r;

	//calculate effect
	vec3 worldPos = MODEL_MATRIX[3].xyz;
	float effect = (sin(TIME*effect_speed + (worldPos.z*distance_period)) - 1.0) * max_displacement;

	//apply effect
	VERTEX.z += mix(-0.125f, 0.0, clamp(distance(worldPos.z, open_locker_pos)/open_locker_influence, 0.0, 1.0));
	VERTEX.z += power * effect;
}

void fragment() {
	vec2 delta = screen_delta(UV, texture_albedo);
	LIGHT_VERTEX = texel_snap(VERTEX, delta);

	vec3 snapped_normal = texel_snap(NORMAL, delta);
	if (length(snapped_normal) > 1e-3)
		NORMAL = normalize(snapped_normal);

	ALBEDO = texture(texture_albedo, UV).rgb;
	NORMAL_MAP = texture(normal_map, UV).xyz;
	ROUGHNESS = roughness;
}
