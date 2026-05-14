export const slugFormat = (pageName: string) => {
  return pageName.split(" ")
    .map((c) => c.toLowerCase())
    .join("-");
}