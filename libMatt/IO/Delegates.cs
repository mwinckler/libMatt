using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.IO {

	public delegate void FileCopyProgressChanged(long filesize, int pctComplete, string message);
	public delegate void FileCopyCompleteDelegate(string sourceFilePath, string destFilePath, long size);

}
