const float TEXEL_SNAP_LIMIT = 1e3;

bool texel_snap_ok(float v) { return abs(v) < TEXEL_SNAP_LIMIT; }
bool texel_snap_ok(vec2 v) { return all(lessThan(abs(v), vec2(TEXEL_SNAP_LIMIT))); }
bool texel_snap_ok(vec3 v) { return all(lessThan(abs(v), vec3(TEXEL_SNAP_LIMIT))); }
bool texel_snap_ok(vec4 v) { return all(lessThan(abs(v), vec4(TEXEL_SNAP_LIMIT))); }

// Calculate screen-space displacement from the center of the current texel.
vec2 screen_delta(vec2 uv, sampler2D tex) {
	vec2 tex_size = vec2(textureSize(tex, 0));
	vec2 texel_center = (floor(uv * tex_size) + 0.5) / tex_size;
	vec2 offset = texel_center - uv;

	vec2 dx = dFdx(uv);
	vec2 dy = dFdy(uv);
	float det = dx.x * dy.y - dy.x * dx.y;

	float scale = max(dot(dx, dx), dot(dy, dy));

	bool valid = texel_snap_ok(dx) && texel_snap_ok(dy) && texel_snap_ok(offset)
		&& abs(det) > (1.0/TEXEL_SNAP_LIMIT) * scale;

	float inv_det = valid ? 1.0 / det : 0.0;
	mat2 uv_to_screen = mat2(vec2(dy.y, -dx.y), vec2(-dy.x, dx.x)) * inv_det;
	vec2 delta = uv_to_screen * offset;

	// Reject bad results
	if (!valid || !texel_snap_ok(delta))
		delta = vec2(0.0);

	return delta;
}

// Snap value to extrapolated value at the center of the current texel.
// Falls back to the unsnapped value if bad result
float texel_snap(float v, vec2 delta) {
	float s = v + dFdx(v) * delta.x + dFdy(v) * delta.y;
	return texel_snap_ok(s) ? s : v;
}

vec2 texel_snap(vec2 v, vec2 delta) {
	vec2 s = v + dFdx(v) * delta.x + dFdy(v) * delta.y;
	return texel_snap_ok(s) ? s : v;
}

vec3 texel_snap(vec3 v, vec2 delta) {
	vec3 s = v + dFdx(v) * delta.x + dFdy(v) * delta.y;
	return texel_snap_ok(s) ? s : v;
}

vec4 texel_snap(vec4 v, vec2 delta) {
	vec4 s = v + dFdx(v) * delta.x + dFdy(v) * delta.y;
	return texel_snap_ok(s) ? s : v;
}
