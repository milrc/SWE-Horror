void make_kernel(inout vec4 n[9], sampler2D tex, vec2 coord, vec2 size)
{
	float w = 1.0 / size.x;//size of a pixel in uv coordinates
	float h = 1.0 / size.y;

	n[0] = texture(tex, coord + vec2( -w, -h));
	n[1] = texture(tex, coord + vec2(0.0, -h));
	n[2] = texture(tex, coord + vec2(  w, -h));
	n[3] = texture(tex, coord + vec2( -w, 0.0));
	n[4] = texture(tex, coord);
	n[5] = texture(tex, coord + vec2(  w, 0.0));
	n[6] = texture(tex, coord + vec2( -w, h));
	n[7] = texture(tex, coord + vec2(0.0, h));
	n[8] = texture(tex, coord + vec2(  w, h));
}

vec4 edge_detection(sampler2D text, vec2 uv, vec2 size) {
	vec4 n[9];
	make_kernel(n, text, uv, size);//gl_TexCoord[0].st = vec2(0)?

	vec4 sobel_edge_h = n[2] + (2.0*n[5]) + n[8] - (n[0] + (2.0*n[3]) + n[6]);
  	vec4 sobel_edge_v = n[0] + (2.0*n[1]) + n[2] - (n[6] + (2.0*n[7]) + n[8]);
	vec4 sobel = sqrt((sobel_edge_h * sobel_edge_h) + (sobel_edge_v * sobel_edge_v));
	return sobel;
}

float draw_edges(sampler2D text, vec2 uv, vec2 size) {
	return 1.0 - rgb_to_oklch(edge_detection(text, uv, size).rgb).x;
}