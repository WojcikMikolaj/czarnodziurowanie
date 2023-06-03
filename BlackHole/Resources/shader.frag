#version 410 core

struct PSInput
{
    vec4 pos;
    vec2 tex;
    vec4 viewVec;
};

in PSInput input;

out vec4 FragColor;

uniform samplerCube cubemap;

uniform float mass;
uniform vec3 cameraPos;

const float eps = 0.0001f;
const int iterCount = 1000;
const float PI = 3.14159265358979f;

float FunctionAtW(float w, float b)
{
    return w * w * (w * 2 * mass / b - 1) + 1;
}

float DerivativeAtW(float w, float b)
{
    return w * (w * 6 * mass / b - 2);
}

float Newton(float x, float b)
{
    return x - FunctionAtW(x, b) / DerivativeAtW(x, b);
}

//Calculate w1
float FindRoot(float b)
{
    float root = 0;
    float lastRoot = 1000;
    while (eps < abs(root - lastRoot))
    {
        lastRoot = root;
        root = Newton(root, b);
    }
    return root;
}

float Integrate(float w1, float b)
{
    float sum = 0;
    float h = w1 / iterCount;
    float hOver2 = h / 2;
    float t = 0;
    for (int i = 0; i < iterCount; i++)
    {
        sum = sum + h * FunctionAtW(t + hOver2, b);
        t = t + h;
    }
    return sum;
}

mat4 GenerateRotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);

    float c = cos(angle);
    float s = sin(angle);

    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    float x2 = axis.x * axis.x;
    float y2 = axis.y * axis.y;
    float z2 = axis.z * axis.z;

    float xy = axis.x * axis.y;
    float xz = axis.x * axis.z;
    float yz = axis.y * axis.z;

    vec4 col1 = vec4(
        c + x2 * (1 - c),
        xy * (1 - c) + z * s,
        xz * (1 - c) - y * s,
        0);

    vec4 col2 = vec4(
        xy * (1 - c) - z * s,
        c + y2 * (1 - c),
        yz * (1 - c) + x * s,
        0);

    vec4 col3 = vec4(
        xz * (1 - c) + y * s,
        yz * (1 - c) - x * s,
        c + z2 * (1 - c),
        0);

    vec4 col4 = vec4(0, 0, 0, 1);

    return mat4(col1, col2, col3, col4);
}

void main()
{
    vec3 cameraVec = cameraPos;
    float b = length(cameraVec - dot(cameraVec, normalize(input.viewVec.xyz)));   
    if (b > sqrt(27.0) * mass)
    {
        float deltaPhi = Integrate(FindRoot(b), b);
        float foldAngle = deltaPhi - PI;

        vec3 rotationAxis = cross(normalize(input.viewVec.xyz), cameraPos);

        mat4 foldMtx = GenerateRotationMatrix(rotationAxis, foldAngle);
        vec4 newViewVec = foldMtx * input.viewVec;
        FragColor = texture(cubemap, normalize(newViewVec.xyz));
    }
    else
    {
        FragColor = texture(cubemap, normalize(input.viewVec.xyz));
    }
}