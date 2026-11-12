// NOTE: Shader automatically converted from Godot Engine 4.7.1.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode blend_mix, depth_draw_opaque, diffuse_burley, specular_schlick_ggx, cull_back;

//uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo : source_color, filter_nearest, repeat_enable;
uniform sampler2D normal_map : hint_normal, filter_nearest;

uniform float roughness : hint_range(0.0, 1.0) = 0.7;
uniform float max_displacement : hint_range(0.0, 1.0) = 0.05;
uniform float max_distance : hint_range(0, 50.0) = 5.0;
uniform float effect_speed = 1.0;
uniform float distance_period = 1.0;
uniform sampler2D distance_to_displacement : repeat_disable;

// Calculate screen-space displacement from the center of the current texel
vec2 screen_delta(vec2 uv, sampler2D tex) {
	vec2 tex_size = vec2(textureSize(tex, 0));
	vec2 texel_center = (floor(uv * tex_size) + 0.5) / tex_size;
	vec2 offset = texel_center - uv;
	mat2 screen_to_uv = mat2(dFdx(uv), dFdy(uv));
	mat2 uv_to_screen = inverse(screen_to_uv);
	return uv_to_screen * offset;
}

// Snap vec_type to extrapolated value at the center of the current texel
float texel_snap(float v, vec2 delta) {
	return v + dFdx(v) * delta.x + dFdy(v) * delta.y;
}

vec2 texel_snap(vec2 v, vec2 delta) {
	return v + dFdx(v) * delta.x + dFdy(v) * delta.y;
}

vec3 texel_snap(vec3 v, vec2 delta) {
	return v + dFdx(v) * delta.x + dFdy(v) * delta.y;
}

vec4 texel_snap(vec4 v, vec2 delta) {
	return v + dFdx(v) * delta.x + dFdy(v) * delta.y;
}

float hash(vec3 p) {
    p = fract(p * 0.3183099 + 0.1);
    p *= 17.0;
    return fract(p.x * p.y * p.z * (p.x + p.y + p.z));
}

void vertex() {
	//calculate power of effect
	float dist = length((MODELVIEW_MATRIX * vec4(VERTEX, 1.0)).xyz); //get distance from camera
	float power = textureLod(distance_to_displacement, vec2(dist/max_distance, 0.0), 0.0).r;

	//calculate effect
	vec3 worldPos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	float effect = (sin(TIME*effect_speed + (worldPos.z*distance_period)) - 1.0) * max_displacement;

	//apply effect
	VERTEX.z += power * effect;
	VERTEX.x += (hash(MODEL_MATRIX[3].xyz))*0.001; //slight random offset to prevent Forward+ crash.
}

void fragment() {
	vec2 delta = screen_delta(UV, texture_albedo);
	LIGHT_VERTEX = texel_snap(VERTEX, delta);
	NORMAL = texel_snap(NORMAL, delta);
	
	vec4 albedo_tex = texture(texture_albedo, UV).rgba;
	ALBEDO = albedo_tex.rgb;
	ALPHA = albedo_tex.a;
	NORMAL_MAP = texture(normal_map, UV).xyz;
	ROUGHNESS = roughness;
    ALPHA_SCISSOR_THRESHOLD = 0.5f;
}
