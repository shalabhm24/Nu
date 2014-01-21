﻿namespace Nu
open System
open SDL2
open OpenTK
open TiledSharp
open Nu
open Nu.Core
open Nu.Constants
open Nu.Sdl
open Nu.Audio
open Nu.Rendering
open Nu.Physics
open Nu.Metadata
open Nu.Entities
open Nu.GroupModule
open Nu.ScreenModule
open Nu.GameModule
open Nu.WorldModule
module OmniBlade =

    // transition literals
    let IncomingTime = 60
    let IdlingTime = 60
    let OutgoingTime = 30

    // splash literals
    let SplashAddress = addr "splash"

    // title literals
    let TitleAddress = addr "title"
    let TitleGroupName = Lun.make "group"
    let TitleGroupAddress = TitleAddress @ [TitleGroupName]
    let TitleGroupFileName = "Assets/OmniBlade/Groups/Title.nugroup"
    let ClickTitleGroupNewAddress = straddrstr "click" TitleGroupAddress "new"
    let ClickTitleGroupLoadAddress = straddrstr "click" TitleGroupAddress "load"
    let ClickTitleGroupCreditsAddress = straddrstr "click" TitleGroupAddress "credits"
    let ClickTitleGroupExitAddress = straddrstr "click" TitleGroupAddress "exit"

    // load literals
    let LoadAddress = addr "load"
    let LoadGroupName = Lun.make "group"
    let LoadGroupAddress = LoadAddress @ [LoadGroupName]
    let LoadGroupFileName = "Assets/OmniBlade/Groups/Load.nugroup"
    let ClickLoadGroupBackAddress = straddrstr "click" LoadGroupAddress "back"

    // credits literals
    let CreditsAddress = addr "credits"
    let CreditsGroupName = Lun.make "group"
    let CreditsGroupAddress = CreditsAddress @ [CreditsGroupName]
    let CreditsGroupFileName = "Assets/OmniBlade/Groups/Credits.nugroup"
    let ClickCreditsGroupBackAddress = straddrstr "click" CreditsGroupAddress "back"

    // field literals
    let FieldAddress = addr "field"
    let FieldGroupName = Lun.make "group"
    let FieldGroupAddress = FieldAddress @ [FieldGroupName]
    let FieldGroupFileName = "Assets/OmniBlade/Groups/Field.nugroup"
    let ClickFieldGroupBackAddress = straddrstr "click" FieldGroupAddress "back"

    // time literals
    let TimeAddress = addr "time"

    let createTitleScreen world =
        let world_ = createDissolveScreenFromFile TitleGroupFileName TitleGroupName IncomingTime OutgoingTime TitleAddress world
        let world_ = subscribe ClickTitleGroupNewAddress [] (handleEventAsScreenTransition TitleAddress FieldAddress) world_
        let world_ = subscribe ClickTitleGroupLoadAddress [] (handleEventAsScreenTransition TitleAddress LoadAddress) world_
        let world_ = subscribe ClickTitleGroupCreditsAddress [] (handleEventAsScreenTransition TitleAddress CreditsAddress) world_
        subscribe ClickTitleGroupExitAddress [] handleEventAsExit world_

    let createLoadScreen world =
        let world' = createDissolveScreenFromFile LoadGroupFileName LoadGroupName IncomingTime OutgoingTime LoadAddress world
        subscribe ClickLoadGroupBackAddress [] (handleEventAsScreenTransition LoadAddress TitleAddress) world'

    let createCreditsScreen world =
        let world' = createDissolveScreenFromFile CreditsGroupFileName CreditsGroupName IncomingTime OutgoingTime CreditsAddress world
        subscribe ClickCreditsGroupBackAddress [] (handleEventAsScreenTransition CreditsAddress TitleAddress) world'

    let createFieldScreen world =
        let world' = createDissolveScreenFromFile FieldGroupFileName FieldGroupName IncomingTime OutgoingTime FieldAddress world
        subscribe ClickFieldGroupBackAddress [] (handleEventAsScreenTransition FieldAddress TitleAddress) world'

    let tryCreateOmniBladeWorld sdlDeps extData =
        let optWorld = tryCreateEmptyWorld sdlDeps extData
        match optWorld with
        | Left _ as left -> left
        | Right world ->
            let playSong = PlaySong { Song = { SongAssetName = Lun.make "Song"; PackageName = Lun.make "Default"; PackageFileName = "AssetGraph.xml" }; FadeOutCurrentSong = true }
            let splashScreenSprite = { SpriteAssetName = Lun.make "Image5"; PackageName = Lun.make "Default"; PackageFileName = "AssetGraph.xml" }
            let world_ = { world with AudioMessages = playSong :: world.AudioMessages }
            let world_ = addSplashScreen (transitionScreenHandler TitleAddress) SplashAddress IncomingTime IdlingTime OutgoingTime splashScreenSprite world_
            let world_ = createTitleScreen world_
            let world_ = createLoadScreen world_
            let world_ = createCreditsScreen world_
            let world_ = createFieldScreen world_
            let world_ = transitionScreen SplashAddress world_
            Right world_