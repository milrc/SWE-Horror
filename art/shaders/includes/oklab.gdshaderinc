/*
Shader from Godot Shaders - the free shader library.
godotshaders.com/shader/oklch-posterization-color-palette-shader

MIT License. Made by Yūgen. Feel free to use, improve and change this shader according to your needs 
and consider sharing the modified result on godotshaders.com.

Inspired by https://gist.github.com/ronniebasak/e5331e54cf9414ab0fec23b4f6a27e2a
And https://www.youtube.com/watch?v=Gjr2CUczDpk

For color palette inspiration, see https://lospec.com/palette-list
*/

vec3 rgb_to_oklch(vec3 rgb) {
	// convert srgb to linear rgb
	vec3 linear_rgb;
	linear_rgb.r = (rgb.r <= 0.04045) ? rgb.r / 12.92 : pow((rgb.r + 0.055) / 1.055, 2.4);
	linear_rgb.g = (rgb.g <= 0.04045) ? rgb.g / 12.92 : pow((rgb.g + 0.055) / 1.055, 2.4);
	linear_rgb.b = (rgb.b <= 0.04045) ? rgb.b / 12.92 : pow((rgb.b + 0.055) / 1.055, 2.4);

	// convert linear rgb to xyz (CIE 1931 XYZ color space) primary color space
	float x = linear_rgb.r * 0.4124 + linear_rgb.g * 0.3576 + linear_rgb.b * 0.1805;
	float y = linear_rgb.r * 0.2126 + linear_rgb.g * 0.7152 + linear_rgb.b * 0.0722;
	float z = linear_rgb.r * 0.0193 + linear_rgb.g * 0.1192 + linear_rgb.b * 0.9505;

	// CIE standard illuminant D65 (color temperature of ~6504K) in the xyz color space
	// For reference: https://en.wikipedia.org/wiki/Standard_illuminant
	float xd65 = 0.95047;
	float yd65 = 1.0;
	float zd65 = 1.08883;

	// normalize xyz color values by the natural daylight factor
	float x_norm = x / xd65;
	float y_norm = y / yd65;
	float z_norm = z / zd65;

	// precompute xyz values for oklab conversion later
	float fx = (x_norm > 0.008856) ? pow(x_norm, 1.0 / 3.0) : (7.787 * x_norm + 16.0 / 116.0);
	float fy = (y_norm > 0.008856) ? pow(y_norm, 1.0 / 3.0) : (7.787 * y_norm + 16.0 / 116.0);
	float fz = (z_norm > 0.008856) ? pow(z_norm, 1.0 / 3.0) : (7.787 * z_norm + 16.0 / 116.0);

	// convert from xyz to oklab
	float l_lab = 116.0 * fy - 16.0;
	float a_lab = 500.0 * (fx - fy);
	float b_lab = 200.0 * (fy - fz);

	// convert from oklab to oklch
	float c = sqrt(a_lab * a_lab + b_lab * b_lab);
	float h = degrees(atan(b_lab, a_lab));
	if (h < 0.0) {
		h += 360.0;
	}

	return vec3(l_lab / 100.0, c / 100.0, h);
}

vec3 oklch_to_rgb(vec3 lch) {
	// scale l and h back to values between 0 and 100 and h to radians from degrees
	float l = lch.x * 100.0;
	float c = lch.y * 100.0;
	float h_rad = radians(lch.z);

	// convert c and h to oklab a and b
	float a_lab = cos(h_rad) * c;
	float b_lab = sin(h_rad) * c;

	// precompute oklab values for xyz conversion later
	float fy = (l + 16.0) / 116.0;
	float fx = a_lab / 500.0 + fy;
	float fz = fy - b_lab / 200.0;
	vec3 f = vec3(fx, fy, fz);
	vec3 f_cubed = pow(f, vec3(3.0));

	// inverse oklab (to xyz) conversion
	vec3 mask_xyz = step(vec3(0.008856), f_cubed);
	vec3 xyz = mask_xyz * f_cubed + (1.0 - mask_xyz) * ((f - vec3(16.0/116.0)) / 7.787);
	vec3 d65 = vec3(0.95047, 1.0, 1.08883);
	xyz *= d65;

	// convert from xyz to linear rgb
	float r = xyz.x *  3.2406 + xyz.y * -1.5372 + xyz.z * -0.4986;
	float g = xyz.x * -0.9689 + xyz.y *  1.8758 + xyz.z *  0.0415;
	float b = xyz.x *  0.0557 + xyz.y * -0.2040 + xyz.z *  1.0570;
	vec3 linear_rgb = vec3(r, g, b);

	// gamma correction mask
	vec3 linear_gamma = linear_rgb * 12.92;
	vec3 non_linear_gamma = 1.055 * pow(linear_rgb, vec3(1.0 / 2.4)) - 0.055;

	// linear rgb to srgb
	vec3 mask_rgb = step(linear_rgb, vec3(0.0031308));
	vec3 srgb = mix(non_linear_gamma, linear_gamma, mask_rgb);

	return clamp(srgb, 0.0, 1.0);
}

float ok_distance_squared(vec3 lch, vec3 lch2) {
	float chroma_distance = mod(((lch.z - lch2.z) + 180.0), 360.0) - 180.0;
	return pow(lch.x - lch.x, 2.0) + pow(lch.y - lch.y, 2.0) + pow(chroma_distance/180.0, 2.0);
}
float ok_distance(vec3 lch, vec3 lch2) {
	return sqrt(ok_distance_squared(lch, lch2));	
}

float ok_distance_weighted_squared(vec3 lch, vec3 lch2, vec3 weights) {
	float chroma_distance = mod(((lch.z - lch2.z) + 180.0), 360.0) - 180.0;
	return pow((lch.x - lch.x)*weights.x, 2.0) + pow((lch.y - lch.y)*weights.y, 2.0) + pow((chroma_distance/180.0)*weights.z, 2.0);
}
float ok_distance_weighted(vec3 lch, vec3 lch2, vec3 weights) {
	return sqrt(ok_distance_weighted_squared(lch, lch2, weights));	
}