shader_type spatial;
render_mode unshaded, fog_disabled, depth_test_disabled;

#include "oklab.gdshaderinc"
#include "palette.gdshaderinc"
#include "sobel.gdshaderinc"

uniform sampler2D depth_texture : source_color, hint_depth_texture;
uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;
uniform sampler2D noise : repeat_disable, filter_nearest;
uniform bool preview_hatch_levels = false;
uniform float hatch_1 : hint_range(0.0, 1.0, 0.005) = 0.375;
uniform float hatch_2 : hint_range(0.0, 1.0, 0.005) = 0.205;
uniform float hatch_3 : hint_range(0.0, 1.0, 0.005) = 0.12;
uniform float hatch_4 : hint_range(0.0, 1.0, 0.005) = 0.01;
uniform float hatch_density = 150;
uniform float hatch_warp = 1.23;
uniform float hatch_thickness = 0.16;
uniform float edge_threshold = -.16;
uniform float edge_thickness = 0.65;
uniform float edge_warp = 0.01;
uniform float steps_l : hint_range(1.0, 50.0, 1.0) = 4;
uniform float level_c : hint_range(0.01, 15.0) = 15.0;
uniform float level_h : hint_range(0.01, 0.1) = 0.05;
uniform float color_boil = 0.01;
uniform sampler2D value_to_thickness : repeat_disable, filter_linear;


vec2 rotateUV(vec2 uv, float r)
{
    float mid = 0.5;
    return vec2(
        cos(r) * (uv.x - mid) + sin(r) * (uv.y - mid) + mid,
        cos(r) * (uv.y - mid) - sin(r) * (uv.x - mid) + mid
    );
}

vec2 get_noise(vec2 uv) {
	return texture(noise, fract(uv)).xy - vec2(0.5f);
}

float hatch(vec2 uv, float step, float value, float thickness, float rot, vec2 offset) {
	vec2 grid_uv = uv;
	grid_uv *= hatch_density;//shrink
	grid_uv = rotateUV(grid_uv, rot);
	grid_uv += get_noise(uv + offset)*hatch_warp; // warp
	grid_uv = fract(grid_uv + offset);//mod

	float hatch = step(value, step);

	float line = step(thickness/2.0, grid_uv.x);
	line *= step(grid_uv.x, 1.0 - thickness/2.0);
	return line*hatch + step(step, value);
}

float posterize(float value, float levels) {
	return round(value * levels) / levels;
}

void vertex() {
    POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	vec3 color;
	vec2 uv = vec2(UV.x, 1.0 - UV.y);
    vec2 ratio_uv = vec2(uv.x, uv.y * VIEWPORT_SIZE.y / VIEWPORT_SIZE.x);
	float value = rgb_to_oklch(texture(screen_texture, uv).xyz).r;

    //Draw outlines
	vec2 edge_uv = uv;
	float edgesDetected = draw_edges(screen_texture, uv + get_noise(uv + vec2(0.325f))*edge_warp, VIEWPORT_SIZE/edge_thickness);
	edgesDetected = step(edge_threshold, edgesDetected); //remove phantom edges

    //Hatch to appropriate values
    float hatching = 1.0;
	float sampled_thickness = mix(hatch_thickness, 1.0, texture(value_to_thickness, vec2(value)).r); // get thickness curve from texture
    hatching *= hatch(ratio_uv, hatch_1, value, sampled_thickness, 3.0*PI/4.0, vec2(0.4f));
    hatching *= hatch(ratio_uv, hatch_2, value, sampled_thickness, PI/4.0, vec2(0));
    hatching *= hatch(ratio_uv, hatch_3, value, sampled_thickness, 0, vec2(0));
    hatching *= hatch(ratio_uv, hatch_4, value, sampled_thickness, PI/2.0, vec2(0.15));

    //Fill color
	vec3 oklch = rgb_to_oklch(texture(screen_texture, uv + get_noise(uv + vec2(0.125f))*color_boil).xyz); //convert to oklab color space
    oklch = vec3(posterize(oklch.r, steps_l), posterize(oklch.g, level_c), posterize(oklch.b, level_h)); //posterize lightness, chroma, and hues
    float oklch_mask = step(0.01, oklch.r + oklch.g); //get anywhere that isn't black
    
	//compose
	color = mix(vec3(1.0), get_nearest_color(oklch_to_rgb(oklch)), oklch_mask) //place colors or white
			*hatching //hatch
			*edgesDetected; //draw lines

	//Just show hatch gray-scale if desired
	if (preview_hatch_levels) {
		float level = 0.0;
		level += step(hatch_1, value)*0.2f;
		level += step(hatch_2, value)*0.2f;
		level += step(hatch_3, value)*0.2f;
		level += step(hatch_4, value)*0.2f;
		color = vec3(level);
	}

	ALBEDO = color;
	DEPTH = 0.0;
}