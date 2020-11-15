# C# Image Converter



## Image Classes

- System.Windows.Media.Imaging.BitmapSource

- System.Windows.Media.Imaging.WriteableBitmap

- System.Drawing.Bitmap

- SixLabors.ImageSharp.Image\<Bgr24\>

- ~~OpenCVSharp.Mat~~

- ~~OpenCV::Mat~~

- MyImagePixels (Bgr24)

  


## Image File Format

- BMP

- JPEG

- PNG

- TIFF

  

## Table

|                        | BitmapSource | Drawing | Image\<Bgr24> | MyImagePixels |
| ---------------------- | ------------ | ------- | ------------- | ------------- |
| From ImageFile         | Done         | Done    | Done          | **Yet**       |
| To ImageFile           | Done         | Done    | Done          | **Done?**     |
| Read Pixels            | Done         | Done    | Done          | Done          |
| To BitmapSource        | -            | Done    | Done          | Done          |
| Update WriteableBitmap | Done         | Done    | Done          | Done          |
| To Drawing.Bitmap      | Done         | -       | Done          | Done          |
| To Image\<Bgr24>       | Done         | Done    | -             | Done          |
| To MyImagePixels       | Done         | Done    | Done          | -             |



