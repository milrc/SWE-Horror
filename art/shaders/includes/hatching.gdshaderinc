#include "oklab.gdshaderinc"
#include "palette.gdshaderinc"
#include "uv_maps.gdshaderinc"

uniform sampler2D noise : repeat_disable, filter_nearest;

group_uniforms Hatching;
uniform float hatch_1 : hint_range(0.0, 1.0, 0.005) = 0.265;
uniform float hatch_2 : hint_range(0.0, 1.0, 0.005) = 0.13;
uniform float hatch_3 : hint_range(0.0, 1.0, 0.005) = 0.045;
uniform float hatch_4 : hint_range(0.0, 1.0, 0.005) = 0.01;
uniform float hatch_density = 300;
uniform float hatch_warp = 0.25;
uniform float hatch_thickness = 0.33;
uniform sampler2D value_to_thickness : repeat_disable, filter_linear, hint_default_black;
uniform bool preview_hatch_levels = false;

group_uniforms Color_Matching;
uniform float color_threshold : hint_range(0, 1.0) = 0.1;
uniform float color_boil = 0.005;
uniform bool preview_colors = false;

group_uniforms;

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

float distance_squared(vec3 p, vec3 p2) {
	vec3 diff = p - p2;
	return dot(diff, diff);
}

//find closest color in oklch color space
vec3 get_nearest_color(vec3 color, vec3 weights) {
	float min_diff = 1e5;
	vec3 nearest_color = vec3(1.0, 0.0, 0.86);
	for (int i = 0; i < COLOR_PALETTE.length(); i++) {
		float curr_dist = ok_distance_squared(rgb_to_oklch(COLOR_PALETTE[i]), color);
		if (curr_dist < min_diff) {
			min_diff = curr_dist;
			nearest_color = COLOR_PALETTE[i];
		}
	}
	return nearest_color;
}
vec3 get_nearest_color(vec3 color) {
	return get_nearest_color(color, vec3(1.0));
}

float posterize(float value, float levels) {
	return round(value * levels) / levels;
}

vec3 hatching_effect(vec3 input_color, vec2 uv) {
	vec3 color;
	float value = rgb_to_oklch(input_color).r;

    //Hatch to appropriate values
    float hatching = 1.0;
	float sampled_thickness = mix(hatch_thickness, 1.0, texture(value_to_thickness, vec2(value)).r); // get thickness curve from texture
    hatching *= hatch(uv, hatch_1, value, sampled_thickness, 3.0*PI/4.0, vec2(0.4f));
    hatching *= hatch(uv, hatch_2, value, sampled_thickness, PI/4.0, vec2(0));
    hatching *= hatch(uv, hatch_3, value, sampled_thickness, 0, vec2(0));
    hatching *= hatch(uv, hatch_4, value, sampled_thickness, PI/2.0, vec2(0.15));

    //Fill color
    vec2 color_uv = clamp(uv, vec2(0.0), vec2(1.0)); //add noise to screen uv
	vec3 oklch = rgb_to_oklch(input_color); //convert screen color to oklab color space

	//compose
	color = mix(get_nearest_color(oklch), vec3(1.0), step(oklch.x, color_threshold)); //paint white or color, based on oklch_mask

	if (!preview_colors) {
		color *= hatching; //apply hatching mask
	}

	//Just show hatch gray-scale if desired
	if (preview_hatch_levels) {
		float level = 0.0;
		level += step(hatch_1, value)*0.2f;
		level += step(hatch_2, value)*0.2f;
		level += step(hatch_3, value)*0.2f;
		level += step(hatch_4, value)*0.2f;
		color = vec3(level);
	}

    return color;
}