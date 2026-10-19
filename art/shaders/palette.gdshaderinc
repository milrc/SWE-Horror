#include "oklab.gdshaderinc"

const vec3 COLOR_PALETTE[8] = vec3[](
	vec3(0.2745098039, 0.1411764705882353, 0.10980392156862745), // #46241c
 	vec3(0.35294117647058826, 0.16470588235294117, 0.12156862745098039), // #5a2a1f
	vec3(0.3607843137254902, 0.23137254901960785, 0.2), // #5c3b33
	vec3(0.5098039215686274, 0.34509803921568627, 0.25098039215686274), // #825840
	vec3(0.34509803921568627, 0.33725490196078434, 0.3215686274509804), // #585652
	vec3(0.4117647058823529, 0.41568627450980394, 0.41568627450980394), // #696a6a
	vec3(0.6274509803921569, 0.6784313725490196, 0.7137254901960784), // #a0adb6
	vec3(1.0, 1.0, 1.0)  // #FFFFFF
);
vec3 get_nearest_color(vec3 color) {
	float min_diff = 100.0;
	vec3 nearest_color = color;
	for (int i = 0; i < COLOR_PALETTE.length(); i++) {
		float curr_dist = distance(rgb_to_oklch(COLOR_PALETTE[i]), rgb_to_oklch(color));
		if (curr_dist < min_diff) {
			min_diff = curr_dist;
			nearest_color = COLOR_PALETTE[i];
		}
	}
	return nearest_color;
}
