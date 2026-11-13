vec2 rotateUV(vec2 uv, float r)
{
    float mid = 0.5;
    return vec2(
        cos(r) * (uv.x - mid) + sin(r) * (uv.y - mid) + mid,
        cos(r) * (uv.y - mid) - sin(r) * (uv.x - mid) + mid
    );
}
vec2 spherical_map(vec3 direction) {
    float u = atan(direction.z, direction.x) / TAU + 0.5;
    float v = asin(direction.y) / PI + 0.5;
    return vec2(u, v);
}

vec2 tunnel_left_uv(vec2 uv) {
    float y = uv.y - 0.5;
    y /= (1.0 - uv.x);
    y += 0.5;
    uv.x = 1.0/(1.0 - uv.x);
    return vec2(uv.x, clamp(0.0, 1.0, y));
}
vec2 tunnel_right_uv(vec2 uv) {
    float y = uv.y - 0.5;
    y /= uv.x;
    y += 0.5;
    uv.x = 1.0/uv.x;
    return vec2(uv.x, clamp(0.0, 1.0, y));
}
vec2 tunnel_top_uv(vec2 uv) {
    float x = uv.x - 0.5;
    x /= (1.0 - uv.y);
    x += 0.5;
    uv.y = 1.0/(1.0 - uv.y);
    return vec2(clamp(0.0, 1.0, x), uv.y);
}
vec2 tunnel_bottom_uv(vec2 uv) {
    float x = uv.x - 0.5;
    x /= uv.y;
    x += 0.5;
    uv.y = 1.0/uv.y;
    return vec2(clamp(0.0, 1.0, x), uv.y);
}
float tunnel_left_mask(vec2 uv) {
    //left wall = left AND right
    float left_wedge = step(uv.x + uv.y, 1.0);
    float right_wedge = step(uv.x + (1.0 - uv.y), 1.0);
    return left_wedge*right_wedge;
}
float tunnel_right_mask(vec2 uv) {
    //right wall = neither left nor right
    float left_wedge = step(uv.x + uv.y, 1.0);
    float right_wedge = step(uv.x + (1.0 - uv.y), 1.0);
    return (1.0 - left_wedge)*(1.0 - right_wedge);
}
float tunnel_top_mask(vec2 uv) {
    //ceil = left AND NOT right
    float left_wedge = step(uv.x + uv.y, 1.0);
    float right_wedge = step(uv.x + (1.0 - uv.y), 1.0);
    return left_wedge*(1.0 - right_wedge);
}
float tunnel_bottom_mask(vec2 uv) {
    //floor = right and NOT left
    float left_wedge = step(uv.x + uv.y, 1.0);
    float right_wedge = step(uv.x + (1.0 - uv.y), 1.0);
    return right_wedge*(1.0 - left_wedge);
}

vec2 tunnel_map(vec2 uv, vec2 offset, vec2 scale) {
    //add all wall segments
    vec2 result = vec2(0.0);
    result += (tunnel_left_uv(vec2(uv.x*2.0, uv.y))+offset)*scale            *tunnel_left_mask(uv);
    result += (tunnel_right_uv(vec2(uv.x*2.0-1.0, uv.y))+offset)*scale       *tunnel_right_mask(uv);
    result += (tunnel_top_uv(vec2(uv.x, uv.y *2.0))+offset.yx)*scale.yx      *tunnel_top_mask(uv);
    result += (tunnel_bottom_uv(vec2(uv.x, uv.y*2.0-1.0))+offset.yx)*scale.yx*tunnel_bottom_mask(uv);
    return result;
}
float tube_depth(vec2 uv) {
    //distance from center of the screen
    return 1.0-distance(uv, vec2(0.5))*2.0;
}
float tunnel_depth(vec2 uv) {
    //distance from center of the screen (in each dimension)
    vec2 depth = vec2(1.0) - abs(uv - vec2(0.5))*2.0;
    return min(depth.x, depth.y); //min of each, rectangle distance
}
float tunnel_ao(vec2 uv) {
    float top_left = smoothstep(0.0, 1.0, uv.x + uv.y);
    top_left *= smoothstep(2.0, 1.0, uv.x + uv.y);

    float bottom_right = smoothstep(0.0, 1.0, uv.x + (1.0 - uv.y));
    bottom_right *= smoothstep(2.0, 1.0, uv.x + (1.0 - uv.y));
    return (1.0 - max(top_left, bottom_right))*2.0;
}