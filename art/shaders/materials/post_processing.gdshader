shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled;

#include "../includes/uv_maps.gdshaderinc"
#include "../includes/hatching.gdshaderinc"

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest, repeat_disable;

void vertex() {
    POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	vec3 color;
	vec2 uv = vec2(UV.x, 1.0 - UV.y);

    vec4 viewRay = INV_PROJECTION_MATRIX * vec4(uv*2.0 - 1.0, -1.0, 1.0); //clip * inverse projection
    viewRay = vec4(viewRay.xy, -1.0, 0.0);
    vec2 sphere_uv = spherical_map(normalize(mat3(INV_VIEW_MATRIX) * viewRay.xyz)); // Transform to camera-surrounding space

    color = hatching_effect(texture(screen_texture, uv + get_noise(sphere_uv + vec2(0.125f))*color_boil).xyz, sphere_uv);

	ALBEDO = color;
	DEPTH = 0.0;
}