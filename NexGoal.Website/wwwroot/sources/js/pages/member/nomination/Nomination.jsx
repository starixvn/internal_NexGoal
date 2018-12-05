import React, { Component } from 'react';
import axios from 'axios';
import { stringify } from 'qs';
import { Scrollbars } from 'react-custom-scrollbars';
import { toast } from 'react-toastify';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import CreateNomination from './CreateNomination';
import EditNomination from './EditNomination';

export default class Nomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominations: [],
            nominationPeriods: [],
            showEditModal: false,
            showDeleteModal: false
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MemberNomination/GetMemberNominations'), {
                params: {
                    memberId: this.props.isMemberDetail ? this.props.nomineeId : null
                }
            }),
            axios(resolveClientUrl('MemberNomination/GetNominationPeriodsForNominator'))
        ];

        Promise.all(promises).then(([nominationResponse, nominationPeriodResponse]) => {
            this.setState({
                nominations: nominationResponse.data,
                nominationPeriods: nominationPeriodResponse.data
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
                    debugger;
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

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                data={performanceRecord}
                onEdit={this.handleEdit}
                onDelete={this.handleDelete}
            />
        );
    }

    render() {
        return (
            <div className="record-list">
                <Scrollbars style={{ height: '100%' }} autoHide>
                    {
                        this.state.nominationPeriods.length > 0 &&
                        <CreateNomination currentUser={this.props.currentUser} employeeOptions={this.props.employeeOptions}
                            nominationPeriods={this.state.nominationPeriods} isEmployeeSelectable={!this.props.isMemberDetail}
                            nomineeId={this.props.nomineeId} onCreateSuccess={this.handleCreateNominationSuccess}
                        />
                    }
                    {this.state.nominations.map(this.renderPerformanceRecord)}
                </Scrollbars>

                {
                    this.state.showEditModal &&
                    <EditNomination showModal={this.state.showEditModal}
                        currentUser={this.props.currentUser}
                        performanceRecord={this.state.editedNomination}
                        nominationPeriods={this.state.nominationPeriods}
                        employeeOptions={this.props.employeeOptions}
                        onCloseModal={this.closeEditModal}
                        onEditSuccess={this.handleEditNominationSuccess}
                        isEmployeeSelectable={!this.props.isMemberDetail}
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