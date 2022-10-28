////////////////////////////////////////////////////////////////////////////
//
// Epoxy template source code.
// Write your own copyright and note.
// (You can use https://github.com/rubicon-oss/LicenseHeaderManager)
//
////////////////////////////////////////////////////////////////////////////

namespace cameraapp.ViewModels

open Epoxy
open FlashCap
open SkiaSharp

[<Sealed; ViewModel>]
type public MainWindowViewModel() as self =
    do
        // A handler for window loaded
        self.Ready <- Command.Factory.create(fun () -> async {
            let devices = new CaptureDevices()
            let deviceDescriptors = devices.enumerateDescriptors() |> Seq.toArray
            let deviceDescriptor = deviceDescriptors.[0]
            let characteristics = deviceDescriptor.Characteristics.[0]

            let! device = deviceDescriptor.openAsync(
                characteristics,
                fun pixelBufferScope -> async {
                    let imageData = pixelBufferScope.Buffer.referImage()
                    let bitmap = SKBitmap.Decode(imageData)
                    do pixelBufferScope.releaseNow()
                    do! UIThread.bind()
                    self.PreviewImage <- bitmap
                })
            self.device <- device

            device.start()

            self.IsEnabled <- true
        })

        // A handler for fetch button
        self.Fetch <- CommandFactory.create(fun () -> async {
            do self.IsEnabled <- false
        })

    [<DefaultValue>]
    val mutable device: CaptureDevice

    member val Ready: Command = null with get, set
    member val IsEnabled: bool = false with get, set
    member val Fetch: Command = null with get, set
    member val PreviewImage: SKBitmap = null with get, set
