import imageio
import sys
import matplotlib.pyplot as plt
import numpy as np
from skimage import measure
import multiprocessing
import os
import glob


def perlin(n, frequency=1):
    lin = np.linspace(0, frequency, n, endpoint=False)
    x, y = np.meshgrid(lin, lin)  # FIX3: I thought I had to invert x and y here but it was a mistake

    return perlin_impl(x, y)


def perlin_impl(x,y):
    # permutation table
    p = np.arange(256,dtype=int)
    np.random.shuffle(p)
    p = np.stack([p,p]).flatten()
    # coordinates of the top-left
    xi = x.astype(int)
    yi = y.astype(int)
    # internal coordinates
    xf = x - xi
    yf = y - yi
    # fade factors
    u = fade(xf)
    v = fade(yf)
    # noise components
    n00 = gradient(p[p[xi]+yi],xf,yf)
    n01 = gradient(p[p[xi]+yi+1],xf,yf-1)
    n11 = gradient(p[p[xi+1]+yi+1],xf-1,yf-1)
    n10 = gradient(p[p[xi+1]+yi],xf-1,yf)
    # combine noises
    x1 = lerp(n00,n10,u)
    x2 = lerp(n01,n11,u) # FIX1: I was using n10 instead of n01
    return lerp(x1,x2,v) # FIX2: I also had to reverse x1 and x2 here


def lerp(a,b,x):
    "linear interpolation"
    return a + x * (b-a)


def fade(t):
    "6t^5 - 15t^4 + 10t^3"
    return 6 * t**5 - 15 * t**4 + 10 * t**3


def gradient(h,x,y):
    "grad converts h to the right gradient vector and return the dot product with (x,y)"
    vectors = np.array([[0,1],[0,-1],[1,0],[-1,0]])
    g = vectors[h%4]
    return g[:,:,0] * x + g[:,:,1] * y


def delete_probability(source):
    if 'Mars' in source:
        return 0.85
    return 0.7


def remove_resources(source, dest):
    # Fix the seed so that in future if we re-run this, we don't change the locations of ores
    np.random.seed(seed_from_source_filename(source))

    image = imageio.imread(source)

    nonzero = image[:, :, 2] != 255
    all_labels, count = measure.label(nonzero, background=0, return_num=True)
    regionprops = measure.regionprops(all_labels)

    noise = np.array(perlin(n=2048, frequency=3))
    min_, max_ = np.min(noise), np.max(noise)
    noise = (noise - min_) / (max_ - min_)

    choose_region_probabilities = [noise[int(rgn.centroid[0]), int(rgn.centroid[1])] for rgn in regionprops]
    choose_region_probabilities = np.array(choose_region_probabilities)
    choose_region_probabilities /= np.sum(choose_region_probabilities)

    probability_to_delete = delete_probability(source)
    to_delete_count = int(probability_to_delete * count)
    to_delete = np.random.choice(np.arange(1, count + 1), to_delete_count, replace=False, p=choose_region_probabilities)
    print('delete_probability', probability_to_delete, 'To delete count', to_delete_count, 'len(to_delete)', len(to_delete))
    # to_delete = np.random.choice(np.arange(1, count), to_delete_count, replace=False)
    delete_mask = np.isin(all_labels, to_delete)

    image_deleted = np.copy(image)
    image_deleted[:, :, 2][delete_mask] = 255

    if dest is not None:
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        imageio.imwrite(dest, image_deleted, 'png')
    else:
        plt.imshow(image[:, :, 2], cmap="binary")
        plt.xlim([0, 800])
        plt.ylim([0, 800])
        plt.figure()
        plt.imshow(image_deleted[:, :, 2], cmap="binary")
        plt.xlim([0, 800])
        plt.ylim([0, 800])
        plt.show()


def discover_files(source_dir):
    return glob.glob(os.path.join(source_dir, '**/*_mat.png'), recursive=True)


def make_dest_filename(source_filename, dest_dir):
    planet_dirname = os.path.basename(os.path.dirname(source_filename))
    mat_filename = os.path.basename(source_filename)
    return os.path.join(dest_dir, planet_dirname, mat_filename)


def string_to_random_seed(s):
    hash = 0
    for c in s:
        hash += ord(c)
        hash = hash & 0xFFFFFFFF
    return hash


def seed_from_source_filename(path):
    dirname, filename = os.path.split(path)
    _, parentname = os.path.split(dirname)
    # Last two levels, e.g. EarthLike/bottom_mat.png
    return string_to_random_seed(os.path.join(parentname, filename))


if __name__ == '__main__':
    source = sys.argv[1]
    if len(sys.argv) > 2:
        dest = sys.argv[2]
    else:
        dest = None

    if os.path.isdir(source):
        if not os.path.isdir(dest):
            raise ValueError('Destination must be a directory if source is a directory')

        sources = discover_files(source)
        dest_files = [make_dest_filename(s, dest) for s in sources]
        pool = multiprocessing.Pool(12)
        pool.starmap(remove_resources, zip(sources, dest_files))
    else:
        remove_resources(source, dest)

