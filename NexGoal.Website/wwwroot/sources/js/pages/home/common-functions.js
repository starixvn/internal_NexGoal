export function loadMorePerformanceRecord(vm, page, callback) {
    const records = vm.state.performanceRecords.slice(0, page * vm.itemPerPage);
    vm.setState({
        displayedPerformanceRecords: records,
        hasMore: records.length < vm.state.performanceRecords.length
    }, () => {
        if (callback && typeof callback === 'function') {
            callback();
        }
    });
}