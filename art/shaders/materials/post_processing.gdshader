shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled;

#include "../includes/uv_maps.gdshaderinc"
#include "../includes/hatching.gdshaderinc"

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest, repeat_disable;

void vertex() {
    POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

vec3 linear_to_srgb(vec3 color) {
    return mix(
        pow(color, vec3(1.0 / 2.4)) * 1.055 - vec3(0.055), 
        color * 12.92, 
        step(color, vec3(0.0031308))
    );
}
vec3 srgb_to_linear(vec3 color) {
    return mix(
        color / 12.92, pow((color + 0.055) / 1.055, 
        vec3(2.4)), 
        step(vec3(0.04045), color)
    );
}

void fragment() {
	vec3 color;
	vec2 uv = vec2(UV.x, 1.0 - UV.y);

    vec4 viewRay = INV_PROJECTION_MATRIX * vec4(uv*2.0 - 1.0, -1.0, 1.0); //clip * inverse projection
    viewRay = vec4(viewRay.xy, -1.0, 0.0);
    vec2 sphere_uv = spherical_map(normalize(mat3(INV_VIEW_MATRIX) * viewRay.xyz)); // Transform to camera-surrounding space

    color = texture(screen_texture, uv + get_noise(sphere_uv + vec2(0.125f))*color_boil).xyz; //sample screen texture
    color = linear_to_srgb(color); //convert from HDR to sRGB
    color = hatching_effect(color, sphere_uv); //apply hatching
    color = srgb_to_linear(color); //convert back to HDR

	ALBEDO = color;
	DEPTH = 0.0;
}