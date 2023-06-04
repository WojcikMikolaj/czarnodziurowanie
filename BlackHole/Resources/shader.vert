#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 tex;

struct PSInput
{
    vec4 pos;
    vec2 tex;
    vec4 viewVec;
};


out PSInput input;

uniform mat4 projMatrix;
uniform mat4 invViewMatrix;
uniform mat4 mvp;

PSInput GetRay()
{
    PSInput o;
    o.tex = 0.5 * (vec2(pos.x, -pos.y) + 1);
    o.pos = vec4(pos.xy, 0.5, 1);

    float projXInv = 1 / projMatrix[0][0];
    float projYInv = 1 / projMatrix[1][1];

    // Conevert point pos from persepctive to camera and world
    vec4 viewVec = vec4(projXInv * pos.x, projYInv * pos.y, -1, 0);
    viewVec = invViewMatrix * viewVec;
    o.viewVec = normalize(viewVec);

    return o;
}

void main() {
    gl_Position = vec4(pos, 1.0);
    input = GetRay(); 
}
