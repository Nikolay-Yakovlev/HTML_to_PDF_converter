$(document).ready(function () {
	let convertedFilesList = $("#convertedFilesList");
	let serverUrl = $("#serverUrl");
	if (serverUrl.val() == "") {
		serverUrl.val("https://localhost:44354");
	}
	let url = serverUrl.val() + "/converter/";

	GetFilesList();

	$("#uploadFileBtn").click(function () {
		HideAllMsg();
		$("#processing").show();
		let inputFile = $("#inputFile");
		if (inputFile[0].files.length > 0 && inputFile[0].files[0].size > 0) {
			var formData = new FormData();
			formData.append('file', inputFile[0].files[0]);
			UploadFile(formData);
		}
		else {
			$("#processing").hide();
			$("#errorfileSize").show();
		}
	});

	$("#delFilesBtn").click(function () {
		HideAllMsg();
		Deleting();
	});

	async function UploadFile(data) {
		let uploadResponse = await fetch(url, {
			method: "POST",
			headers: { "Accept": "application/json" },
			body: data,
		});
		if (uploadResponse.ok) {
			$("#processing").hide();
			GetFilesList();
		}
		if (!uploadResponse.ok) {
			$("#processing").hide();
			$("#errorConverting").show();
			GetFilesList();
		}
	};

	async function Deleting() {
		$("#deleting").show();
		convertedFilesList.children().remove();
		let filesDeleteResponse = await fetch(url, {
			method: "DELETE",
			headers: { "Accept": "application/json" }
		});
		HideAllMsg();
		if (filesDeleteResponse.ok) {
			$("#delFilesBtn").hide();}
		if (!filesDeleteResponse.ok) {
			$("#errorDeleteFiles").show();
		}
	}

	async function GetFilesList() {
		convertedFilesList.children().remove();
		$("#loadingFileList").show();
		let filesListResponse = await fetch(url, {
			method: "GET",
			headers: { "Accept": "application/json" }
		});
		HideAllMsg();
		if (filesListResponse.ok) {
			let filesList = await filesListResponse.json();
			if (filesList.length > 0) {
				$("#delFilesBtn").show();
				filesList.forEach(el => convertedFilesList.append("<p><a href='" + url + el.id + "' id=\"fileLink\">" + el.name + "</a></p>"));
			}
			else $("#delFilesBtn").hide();
		}
		if (!filesListResponse.ok) {
			$("#errorLoadFileList").show();
		}
	}

	function HideAllMsg() {
		$("#processing").hide();
		$("#errorConverting").hide();
		$("#loadingFileList").hide();
		$("#errorLoadFileList").hide();
		$("#errorfileSize").hide();
		$("#deleting").hide();
		$("#errorDeleteFiles").hide();
	}
});
