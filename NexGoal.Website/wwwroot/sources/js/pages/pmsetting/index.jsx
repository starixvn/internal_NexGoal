import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './Table';
import AddMember from './AddMember';

export default class PMSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                pms: []
            },
            pmOptions: [],
            employeeOptions: [],
            pms: [],
            tableData: [],
            addMember: {
                showModal: false,
                pm: null,
                memberOptions: []
            }
        }
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('common/GetEmployees')),
            axios(resolveClientUrl('common/GetEmployees'), {
                params: {
                    groupIds: 13
                }
            }),
        ];

        Promise.all(promises)
            .then(([employeeResponse, pmResponse]) => {
                let pms = pmResponse.data;
                let pmOptions = pms.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                let employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                let filter = {
                    pms: pmOptions
                };

                this.setState({ filter: filter, pmOptions: pmOptions, pms: pms, employeeOptions: employeeOptions }, () => this.getPMSetting(this.state.filter.pms.map(e => e.value).toString()));
            }).catch(axiosError);
    }

    getPMSetting = (pmIds) => {
        axios.get(resolveClientUrl('PMSetting/GetSetting'), {
            params: {
                pmIds: pmIds
            }
        }).then(response => {
            let data = response.data;
            let selectedPMs = this.state.pms.filter(pm => this.state.filter.pms.some(e => e.value === pm.employeeId));

            this.setState({
                tableData: selectedPMs.map(pm => ({
                    pm: pm,
                    members: data.filter(e => e.projectManagerId === pm.employeeId)
                }))
            });
        }).catch(axiosError);
    }

    handleAddMemberSuccess = () => {
        this.getPMSetting(this.state.addMember.pm.employeeId);
        this.handleCloseAddMemberModal();
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: { ...this.state.filter, ...{ [name]: value } }
        });
    }

    handleAddMember = pm => {
        let existingMembers = this.state.tableData.find(data => data.pm.employeeId === pm.employeeId).members

        this.setState({
            addMember: {
                showModal: true,
                pm: pm,
                memberOptions: this.state.employeeOptions.filter(e => !existingMembers.some(em => em.employeeId === e.value))
            }
        });
    }

    handleCloseAddMemberModal = () => {
        this.setState({ addMember: { ... this.state.addMember, ...{ showModal: false } } });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('PMSetting')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        pmOptions={this.state.pmOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.getPMSetting}
                    />
                    <Table tableData={this.state.tableData} onAddMember={this.handleAddMember} />
                </div>
                <AddMember pm={this.state.addMember.pm} memberOptions={this.state.addMember.memberOptions} showModal={this.state.addMember.showModal}
                    closeModal={this.handleCloseAddMemberModal} onAddMemberSuccess={this.handleAddMemberSuccess} />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<PMSetting />, document.getElementById("pm-setting-wrapper"));