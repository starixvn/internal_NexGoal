import React, { Component } from 'react';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import CustomScrollbars from '../../../shared/components/CustomScrollbars';
import CreateNomination from './CreateNomination';
import EditNomination from './EditNomination';

export default class Nomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominations: [],
            showEditModal: false,
            showDeleteModal: false
        };

        this.employeeOptions = [];
        this.nominationPeriods = [];
        this.nominationPeriodOptions = [];
    }

    componentDidMount() {
        let promises = [
            axios(resolveClientUrl('Common/GetEmployees')),
            axios(resolveClientUrl('MemberNomination/GetNominationPeriodsForNominator')),
            axios(resolveClientUrl('MemberNomination/GetMemberNominations'))
        ];

        if (this.props.isMemberDetails) {
            promises = [
                axios(resolveClientUrl('Common/GetEmployees')),
                axios(resolveClientUrl('MemberNomination/GetNominationPeriodsForNominator')),
                axios(resolveClientUrl('MemberNomination/GetMemberNominationById'), {
                    params: {
                        memberId: this.props.nomineeId
                    }
                })
            ];
        }

        Promise.all(promises).then(([employeeResponse, nominationPeriodResponse, nominationResponse]) => {
            this.employeeOptions = employeeResponse.data.map(member => {
                return { value: member.employeeId, label: `${member.nickName} (${member.fullName})` };
            });

            this.nominationPeriods = nominationPeriodResponse.data;

            this.nominationPeriodOptions = nominationPeriodResponse.data.map(period => {
                return { value: period.nominationPeriodId, label: period.title, quarter: period.quarter };
            });

            this.setState({
                nominations: nominationResponse.data
            });
        }).catch(axiosError);
    }

    handleCreateNominationSuccess = performanceRecordId => {
        axios.get(resolveClientUrl('MemberNomination/GetByRecordId'), {
            params: {
                performanceRecordId: performanceRecordId
            }
        }).then(response => {
            this.setState(prevState => {
                let nominations = [response.data, ...prevState.nominations];

                return {
                    nominations: nominations
                };
            });
        }).catch(axiosError);
    }

    handleEditNominationSuccess = performanceRecordId => {
        axios.get(resolveClientUrl('MemberNomination/GetByRecordId'), {
            params: {
                performanceRecordId: performanceRecordId
            }
        }).then(response => {
            this.setState(prevState => {
                let nominations = prevState.nominations.slice();
                let index = nominations.findIndex(x => x.performanceRecordId === performanceRecordId);
                nominations[index] = response.data;

                return {
                    nominations: nominations,
                    showEditModal: false
                };
            });
        }).catch(axiosError);
    }

    handleEdit = performanceRecordId => {
        this.setState(prevState => ({
            showEditModal: true,
            editedNomination: prevState.nominations.find(x => x.performanceRecordId === performanceRecordId)
        }));
    }

    handleDelete = performanceRecordId => {
        this.setState({
            showDeleteModal: true,
            deletedRecordId: performanceRecordId
        });
    }

    deleteNomination = () => {
        axios({
            url: resolveClientUrl(`MemberNomination/Delete`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: this.state.deletedRecordId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.setState(prevState => {
                    let nominations = prevState.nominations.slice();
                    let index = nominations.findIndex(x => x.performanceRecordId === this.state.deletedRecordId);
                    nominations.splice(index, 1);

                    return {
                        nominations: nominations,
                        deletedRecordId: null,
                        showDeleteModal: false
                    };
                });
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    closeEditModal = () => this.setState({ showEditModal: false, editedNomination: null });

    closeDeleteModal = () => this.setState({ showDeleteModal: false, deletedRecordId: null });

    renderPerformanceRecord = performanceRecord => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                pageType={this.props.pageType}
                data={performanceRecord}
                onEdit={this.handleEdit}
                onDelete={this.handleDelete}
            />
        );
    }

    render() {
        return (
            <div className="record-list">
                <CustomScrollbars>
                    {
                        this.nominationPeriodOptions.length > 0 &&
                        <CreateNomination
                            currentUser={this.props.currentUser}
                            employeeOptions={this.employeeOptions}
                            nominationPeriodOptions={this.nominationPeriodOptions}
                            isEmployeeSelectable={!this.props.isMemberDetails}
                            nomineeId={this.props.nomineeId}
                            onCreateSuccess={this.handleCreateNominationSuccess}
                        />
                    }
                    {this.state.nominations.map(this.renderPerformanceRecord)}
                </CustomScrollbars>

                {
                    this.state.showEditModal &&
                    <EditNomination showModal={this.state.showEditModal}
                        currentUser={this.props.currentUser}
                        performanceRecord={this.state.editedNomination}
                        nominationPeriodOptions={this.nominationPeriodOptions}
                        employeeOptions={this.employeeOptions}
                        onCloseModal={this.closeEditModal}
                        onEditSuccess={this.handleEditNominationSuccess}
                        isEmployeeSelectable={!this.props.isMemberDetails}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal showModal={this.state.showDeleteModal}
                        onCloseModal={this.closeDeleteModal}
                        action="delete"
                        name="this nomination"
                        onSubmit={this.deleteNomination}
                    />
                }
            </div>
        );
    }
}