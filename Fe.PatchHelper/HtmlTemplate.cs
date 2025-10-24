using Fe.PatchHelper.Models;

namespace Fe.PatchHelper;

public class HtmlTemplate
{
    public static string BaseTemplate(SeedMetadata metadata, string patchString) => $$"""
<html>
<head>
    <title>FF4FE: {{metadata.Flags}} / {{metadata.Seed}}</title>
    <link rel="stylesheet" href="https://info.tellah.life/seeds/seed.css">
</head>

<body>
    <div id="main">
        <div id="heading">
            <div class="label">Flags:</div>
            <div id="flags">
                <div>{{metadata.Flags}}</div>
            </div>
            <div class="label">Seed:</div>
            <div id="seed">{{metadata.Seed}}</div>
            <div class="label">Version:</div>
            <div id="version">{{metadata.Version}}</div>
            <div class="label">Check:</div>
            <div>
                <div id="checksum-container">
                    <img class="checksum-tile" src="https://info.tellah.life/img/checksum-{{metadata.Verification.First()}}.png" alt="${{metadata.Verification.First()}}" title="${{metadata.Verification.First()}}">
                    <img class="checksum-tile" src="https://info.tellah.life/img/checksum-{{metadata.Verification.Skip(1).First()}}.png" alt="${{metadata.Verification.Skip(1).First()}}" title="${{metadata.Verification.Skip(1).First()}}">
                    <img class="checksum-tile" src="https://info.tellah.life/img/checksum-{{metadata.Verification.Skip(2).First()}}.png" alt="${{metadata.Verification.Skip(2).First()}}" title="${{metadata.Verification.Skip(2).First()}}">
                    <img class="checksum-tile" src="https://info.tellah.life/img/checksum-{{metadata.Verification.Skip(3).First()}}.png" alt="${{metadata.Verification.Skip(3).First()}}" title="${{metadata.Verification.Skip(3).First()}}">
                </div>
            </div>
        </div>

        <div id="patch">
            <div id="patch_header">Please provide an uncompressed ROM of Final Fantasy II US v1.1:</div>
            <div id="file_section">
                <div id="cached_file" style="display:none;">
                    Loaded <span id="cached_file_name">ROM data</span> from local cache. <span
                        id="clear_cached_file_button" onclick="clearCachedFile();">clear</span>
                </div>
                <input type="file" id="source_rom" onchange="handleRomFileChanged();">
                <div id="apply_button" class="disabled" onclick="tryPatch();">Apply</div>
            </div>
            <div id="error"></div>
        </div>
        <div id="share">
            This seed can be shared by saving and distributing <a href=""
                download="FF4FE.{{metadata.BinaryFlags}}.{{metadata.Seed}}.html">this HTML file</a>, or by
            sharing this <a href="javascript:;" onclick="downloadBpsPatch();">BPS patch</a>.
        </div>
    </div>

    <div id="credit">
        BPS patch creation and application code by <a href="https://github.com/Alcaro" target="_blank">Alcaro</a>. This patch page has be recreated from the pached FE ROM.
    </div>

    <script>

        var INPUT_FILE_ERROR = "The provided ROM is incorrect; please verify you have the correct version of the game and try again.";
        var bpsBytes = null;

        function bytesToBase64(bytes) {
            var binary = '';
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }

        function loadBase64(string) {
            let decoded = atob(string);
            let bytes = new Uint8Array(decoded.length);
            for (var i = 0; i < decoded.length; i++) {
                bytes[i] = decoded.charCodeAt(i);
            }

            return bytes;
        }

        function loadBpsPatchData() {
            if (bpsBytes === null) {
                bpsBytes = loadBase64(RAW_PATCH);
            }
        }

        function downloadBpsPatch() {
            loadBpsPatchData();
            download(bpsBytes, "FF4FE.{{metadata.BinaryFlags}}.{{metadata.Seed}}.bps", "application/octet-stream");
        }

        function handleRomFileChanged() {
            updateFileControls();
        }

        function tryPatch() {
            loadBpsPatchData();
            document.getElementById("error").style.display = "none";

            var cachedRomData = window.localStorage.getItem('cached_rom_data');
            if (cachedRomData != null) {
                var romData = {
                    bytes: loadBase64(cachedRomData),
                    name: window.localStorage.getItem('cached_rom_name'),
                    mime: window.localStorage.getItem('cached_rom_mime')
                };
                continuePatch(romData, false);
            }
            else {
                var romFileElem = document.getElementById("source_rom");
                if (romFileElem.files.length != 1) {
                    return;
                }

                var romFile = romFileElem.files[0];
                var romReader = new FileReader();
                romReader.onload = function () {
                    var romData = { bytes: new Uint8Array(this.result), name: romFile.name, mime: romFile.type };
                    continuePatch(romData, true);
                };
                romReader.readAsArrayBuffer(romFile);
            }
        }

        function continuePatch(romData, shouldCacheRom) {
            try {
                var ret;
                try {
                    ret = applyBps(romData.bytes, bpsBytes);
                } catch (e) {
                    if (e === INPUT_FILE_ERROR) {
                        var beheadedRom = romData.bytes.subarray(512);
                        // maybe a headered rom? skip first 512 bytes for patching
                        ret = applyBps(beheadedRom, bpsBytes);
                        // if we reached here, there were no errors, so the assumption about a headered rom was correct.
                    }
                    else throw e;
                }
                var basename = "FF4FE.{{metadata.BinaryFlags}}.{{metadata.Seed}}";
                var ext = '.' + romData.name.split(".").pop();
                download(ret, basename + ext, romData.mime);

                if (shouldCacheRom) {
                    window.localStorage.setItem('cached_rom_data', bytesToBase64(romData.bytes));
                    window.localStorage.setItem('cached_rom_name', romData.name);
                    window.localStorage.setItem('cached_rom_mime', romData.mime);
                }

            } catch (e) {
                if (typeof (e) == 'string') {
                    document.getElementById("error").textContent = e;
                    document.getElementById("error").style.display = "block";
                }
                else {
                    throw e;
                }
            }
        }

        function clearCachedFile() {
            window.localStorage.removeItem('cached_rom_data');
            window.localStorage.removeItem('cached_rom_file');
            window.localStorage.removeItem('cached_rom_mime');
            updateFileControls();
        }

        function showFlagInfo() {
            document.getElementById('flag_info_container').className = "visible";
        }

        function hideFlagInfo() {
            document.getElementById('flag_info_container').className = "";
        }

        function toggleDisabledFlags() {
            var visible = document.getElementById("showDisabledFlags").checked;
            for (var i = 0; i < DISABLED_FLAG_ELEMENT_IDS.length; i++) {
                var elemId = DISABLED_FLAG_ELEMENT_IDS[i];
                var elem = document.getElementById(elemId);
                if (visible) {
                    addClassName(elem, "shown");
                }
                else {
                    removeClassName(elem, "shown");
                }
            }
        }

        function toggleFlagDescription(flag) {
            var elem = document.getElementById("flag_description_" + flag);
            if (elem) {
                if (elem.style.display == 'block') {
                    elem.style.display = null;
                }
                else {
                    elem.style.display = 'block';
                }
            }
        }

        function addClassName(elem, className) {
            if (typeof (elem) === 'string') {
                elem = document.getElementById(elem);
            }

            if (elem) {
                var classNames = elem.className.split(" ");
                if (classNames.indexOf(className) === -1) {
                    elem.className += " " + className;
                }
            }
        }

        function removeClassName(elem, className) {
            if (typeof (elem) === 'string') {
                elem = document.getElementById(elem);
            }

            if (elem) {
                var classNames = elem.className.split(" ");
                var index = classNames.indexOf(className);
                if (index !== -1) {
                    classNames.splice(index, 1);
                    elem.className = classNames.join(" ");
                }
            }
        }

        function updateFileControls() {
            var hasCachedRom = (window.localStorage.getItem('cached_rom_data') !== null);
            document.getElementById('cached_file').style.display = (hasCachedRom ? 'block' : 'none');
            document.getElementById('source_rom').style.display = (hasCachedRom ? 'none' : 'block');

            var cached_rom_name = window.localStorage.getItem('cached_rom_name');
            if (cached_rom_name) {
                var romNameElem = document.getElementById('cached_file_name');
                romNameElem.innerText = cached_rom_name;
            }

            var applyEnabled = false;
            if (hasCachedRom) {
                applyEnabled = true;
            }
            else {
                var romFileElem = document.getElementById("source_rom");
                applyEnabled = (romFileElem.files.length == 1);
            }

            var applyButtonElem = document.getElementById("apply_button");
            applyButtonElem.className = (applyEnabled ? '' : 'disabled');
        }

        window.onload = function () {
            updateFileControls();
        }
    </script>

    <script type="text/javascript">
        // BPS patch code source: https://media.smwcentral.net/Alcaro/bps/
        // author: https://www.smwcentral.net/?p=profile&id=1686
        // smc support by randomdude999 (https://smwc.me/u/32552)

        //no error checking, other than BPS signature, input size/crc and JS auto checking array bounds
        function applyBps(rom, patch) {
            function crc32(bytes) {
                var c;
                var crcTable = [];
                for (var n = 0; n < 256; n++) {
                    c = n;
                    for (var k = 0; k < 8; k++) {
                        c = ((c & 1) ? (0xEDB88320 ^ (c >>> 1)) : (c >>> 1));
                    }
                    crcTable[n] = c;
                }

                var crc = 0 ^ (-1);
                for (var i = 0; i < bytes.length; i++) {
                    crc = (crc >>> 8) ^ crcTable[(crc ^ bytes[i]) & 0xFF];
                }
                return (crc ^ (-1)) >>> 0;
            };

            var patchpos = 0;
            function u8() { return patch[patchpos++]; }
            function u32at(pos) { return (patch[pos + 0] << 0 | patch[pos + 1] << 8 | patch[pos + 2] << 16 | patch[pos + 3] << 24) >>> 0; }

            function decode() {
                var ret = 0;
                var sh = 0;
                while (true) {
                    var next = u8();
                    ret += (next ^ 0x80) << sh;
                    if (next & 0x80) return ret;
                    sh += 7;
                }
            }

            function decodes() {
                var enc = decode();
                var ret = enc >> 1;
                if (enc & 1) ret = -ret;
                return ret;
            }

            if (u8() != 0x42 || u8() != 0x50 || u8() != 0x53 || u8() != 0x31) throw "Patch is not BPS format. (This should be impossible...)";
            if (decode() != rom.length) throw INPUT_FILE_ERROR;
            if (crc32(rom) != u32at(patch.length - 12)) throw INPUT_FILE_ERROR;

            var out = new Uint8Array(decode());
            var outpos = 0;

            var metalen = decode();
            patchpos += metalen; // can't join these two, JS reads patchpos before calling decode

            var SourceRead = 0;
            var TargetRead = 1;
            var SourceCopy = 2;
            var TargetCopy = 3;

            var inreadpos = 0;
            var outreadpos = 0;

            while (patchpos < patch.length - 12) {
                var thisinstr = decode();
                var len = (thisinstr >> 2) + 1;
                var action = (thisinstr & 3);

                switch (action) {
                    case SourceRead:
                        {
                            for (var i = 0; i < len; i++) {
                                out[outpos] = rom[outpos];
                                outpos++;
                            }
                        }
                        break;
                    case TargetRead:
                        {
                            for (var i = 0; i < len; i++) {
                                out[outpos++] = u8();
                            }
                        }
                        break;
                    case SourceCopy:
                        {
                            inreadpos += decodes();
                            for (var i = 0; i < len; i++) out[outpos++] = rom[inreadpos++];
                        }
                        break;
                    case TargetCopy:
                        {
                            outreadpos += decodes();
                            for (var i = 0; i < len; i++) out[outpos++] = out[outreadpos++];
                        }
                        break;
                }
            }

            return out;
        }

        function download(data, fname, mime) {
            var a = document.createElement("a");
            a.href = URL.createObjectURL(new Blob([data], { type: mime || "application/octet-stream" }));
            a.setAttribute("download", fname);
            a.style.display = "none";
            document.body.appendChild(a);
            setTimeout(function () {
                a.click();
                document.body.removeChild(a);
                setTimeout(function () { self.URL.revokeObjectURL(a.href); }, 250);
            }, 66);
        }

        var RAW_PATCH = "{{patchString}}";
    </script>
</body>
</html>
""";
}
