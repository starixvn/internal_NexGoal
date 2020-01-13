import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import SearchForm from './SearchForm';
import Table from './table/index';
import AddMember from './AddMember';

export default class PMSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                pms: [],
                employee: null
            },
            pmOptions: [],
            employeeOptions: [],
            pms: [],
            tableData: [],
            addMember: {
                showModal: false,
                pm: null,
                memberOptions: []
            },
            removeMember: {
                showModal: false,
                member: {}
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('common/GetEmployees')),
            axios(resolveClientUrl('PMSetting/GetProjectManagers'))
        ];

        Promise.all(promises)
            .then(([employeeResponse, pmResponse]) => {
                const pms = pmResponse.data;
                const pmOptions = pms.map(pm => {
                    return {
                        value: pm.employeeId,
                        label: `${pm.nickName} (${pm.fullName})`
                    };
                });

                const employeeOptions = employeeResponse.data.map(employee => {
                    return {
                        value: employee.employeeId,
                        label: `${employee.nickName} (${employee.fullName})`
                    };
                });

                const filter = {
                    pms: pmOptions
                };

                this.setState({ filter: filter, pmOptions: pmOptions, pms: pms, employeeOptions: employeeOptions }, () => {
                    const newFilter = this.state.filter;
                    this.getPMSetting(newFilter.pms.map(e => e.value).toString(), newFilter.employee ? newFilter.employee.value : null, this.updateTableData);
                });
            }).catch(axiosError);
    }

    updateTableData = response => {
        const data = response.data;
        const selectedPMs = this.state.pms.filter(pm => this.state.filter.pms.some(e => e.value === pm.employeeId));

        this.setState({
            tableData: selectedPMs.map(pm => ({
                pm: pm,
                members: data.filter(e => e.projectManagerId === pm.employeeId)
            }))
        });
    }

    getPMSetting = (projectManagerIds, employeeId, callback) => {
        axios.get(resolveClientUrl('PMSetting/GetSetting'), {
            params: {
                projectManagerIds: projectManagerIds,
                employeeId: employeeId
            }
        }).then(callback).catch(axiosError);
    }

    handleAddMemberSuccess = () => {
        this.getPMSetting(this.state.addMember.pm.employeeId, null, response => {
            const data = response.data;
            const tableData = this.state.tableData.slice();
            const pmSetting = tableData.find(e => e.pm.employeeId === this.state.addMember.pm.employeeId);
            pmSetting.members = data;
            this.setState({ tableData: tableData });
        });
        this.handleCloseAddMemberModal();
    }

    handleFilterSubmit = () => {
        const filter = this.state.filter;
        this.getPMSetting(filter.pms.map(e => e.value).toString(), filter.employee ? filter.employee.value : null, this.updateTableData);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: { ...this.state.filter, ...{ [name]: value } }
        });
    }

    handleAddMember = pm => {
        this.getPMSetting(pm.employeeId, null, response => {
            this.setState({
                addMember: {
                    showModal: true,
                    pm: pm,
                    memberOptions: this.state.employeeOptions.filter(e => !response.data.some(em => em.employeeId === e.value) && e.value !== pm.employeeId)
                }
            });
        });
    }

    handleRemoveMember = member => {
        this.setState({
            removeMember: {
                showModal: true,
                member: member
            }
        });
    }

    handleRemoveMemberConfirmation = () => {
        axios({
            url: resolveClientUrl(`PMSetting/RemoveMember`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                projectManagerId: this.state.removeMember.member.projectManagerId,
                memberId: this.state.removeMember.member.employeeId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('RemovedSuccessfully'));

                const tableData = this.state.tableData.slice();
                const pmSetting = tableData.find(e => e.pm.employeeId === this.state.removeMember.member.projectManagerId);
                const index = pmSetting.members.findIndex(m => m.employeeId === this.state.removeMember.member.employeeId);
                pmSetting.members.splice(index, 1);
                this.setState({ tableData: tableData });
                this.handleCloseRemoveMemberModal();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleCloseAddMemberModal = () => {
        this.setState({ addMember: { ... this.state.addMember, ...{ showModal: false } } });
    }

    handleCloseRemoveMemberModal = () => {
        this.setState({ removeMember: { ... this.state.removeMember, ...{ showModal: false } } });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PMAssignment')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        pmOptions={this.state.pmOptions}
                        employeeOptions={this.state.employeeOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.handleFilterSubmit}
                    />
                    <Table tableData={this.state.tableData} onAddMember={this.handleAddMember} onRemoveMember={this.handleRemoveMember} />
                </div>
                <AddMember pm={this.state.addMember.pm}
                    memberOptions={this.state.addMember.memberOptions}
                    showModal={this.state.addMember.showModal}
                    closeModal={this.handleCloseAddMemberModal}
                    onAddMemberSuccess={this.handleAddMemberSuccess}
                />
                <ToastLayout />
                <ConfirmationModal action="remove"
                    name={this.state.removeMember.member.nickName}
                    showModal={this.state.removeMember.showModal}
                    onCloseModal={this.handleCloseRemoveMemberModal}
                    onSubmit={this.handleRemoveMemberConfirmation}
                />
            </div>
        );
    }
}

ReactDOM.render(<PMSetting />, document.getElementById('pm-setting-wrapper'));