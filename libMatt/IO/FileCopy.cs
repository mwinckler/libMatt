using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace libMatt.IO {

	public class FileCopy {

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
		   CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel,
		   CopyFileFlags dwCopyFlags);

		public delegate CopyProgressResult CopyProgressRoutine(
			long TotalFileSize,
			long TotalBytesTransferred,
			long StreamSize,
			long StreamBytesTransferred,
			uint dwStreamNumber,
			CopyProgressCallbackReason dwCallbackReason,
			IntPtr hSourceFile,
			IntPtr hDestinationFile,
			IntPtr lpData);

		int pbCancel;

		private class FileCopyArgs {
			public string SourceFilename { get; set; }
			public string DestFilename { get; set; }
		}


		public void XCopy(string oldFile, string newFile, FileCopyProgressChanged progressChanged, FileCopyCompleteDelegate completed) {
			var worker = new BackgroundWorker();
			var oldFileInfo = new FileInfo(oldFile);
			if (!oldFileInfo.Exists)
				return;

			long totalSize = oldFileInfo.Length;

			worker.DoWork += (sndr, ev) => {
				var args = ev.Argument as FileCopyArgs;

				CopyFileEx(
					oldFile,
					newFile,
					(fileSize, transferred, streamSize, streamTransferred, dwStreamNumber, dwCallbackReason, hSourceFile, hDestFile, lpData) => {
						int complete = (int)Math.Round(((double)transferred / fileSize) * 100);
						(sndr as BackgroundWorker).ReportProgress(complete, fileSize);
						return CopyProgressResult.PROGRESS_CONTINUE;
					},
					IntPtr.Zero,
					ref pbCancel,
					CopyFileFlags.COPY_FILE_RESTARTABLE
				);

			};
			if (progressChanged != null) {
				worker.ProgressChanged += (sndr, ev) => {
					if (progressChanged != null) {
						progressChanged((long)ev.UserState, ev.ProgressPercentage, "Copying " + Path.GetFileName(oldFile) + ": " + ev.ProgressPercentage + "%");
					}
				};
			}
			if (completed != null) {
				worker.RunWorkerCompleted += (sndr, ev) => {
					if (completed != null) {
						completed(oldFile, newFile, totalSize);
					}
				};
			}

			worker.WorkerReportsProgress = true;
			worker.RunWorkerAsync(new FileCopyArgs() {
				SourceFilename = oldFile,
				DestFilename = newFile
			});

		}

		private CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long StreamByteTrans, uint dwStreamNumber, CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData) {
			return CopyProgressResult.PROGRESS_CONTINUE;
		}

	}

	public enum CopyProgressResult : uint {
		PROGRESS_CONTINUE = 0,
		PROGRESS_CANCEL = 1,
		PROGRESS_STOP = 2,
		PROGRESS_QUIET = 3
	}

	public enum CopyProgressCallbackReason : uint {
		CALLBACK_CHUNK_FINISHED = 0x00000000,
		CALLBACK_STREAM_SWITCH = 0x00000001
	}

	[Flags]
	public enum CopyFileFlags : uint {
		COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
		COPY_FILE_RESTARTABLE = 0x00000002,
		COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
		COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
	}
}
