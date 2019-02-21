import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../../common-functions';
import getResource from '../../../../resourceHelper';
import Rows from './Rows';
import EditMemberPA from '../EditPAForm';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowEditModal: false,
            paRecordId: null
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(nextProps.data) !== JSON.stringify(this.props.data)
            || JSON.stringify(nextProps.gradeOptions) !== JSON.stringify(this.props.gradeOptions)
            || nextState.isShowModal !== this.state.isShowEditModal
            || nextState.paRecordId !== this.state.paRecordId) {
            return true;
        }

        return false;
    }

    onQuickEditPA = (paRecordId, isRevised) => {
        axios({
            url: resolveClientUrl('Review/QuickEdit'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: paRecordId,
                isRevised: isRevised
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.isSuccessful) {
                this.props.onRevisedSuccess(paRecordId, isRevised);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    onClickEditPA = (paRecordId) => {
        this.setState({
            isShowEditModal: true,
            paRecordId: paRecordId
        });
    }

    onCloseModal = () => {
        this.setState({
            isShowEditModal: false,
            paRecordId: null
        });
    }

    onEditSucess = () => {
        this.onCloseModal();
        this.props.renderPA();
    }

    renderRow = () => {
        const isNoData = this.props.data.length === 0;
        if (isNoData) {
            return (
                <tr>
                    <td className="empty-result" colSpan={7}>{getResource('NoRecordFound')}</td>
                </tr>
            );
        } else {
            return (
                this.props.data.map(group => {
                    return (
                        <Rows key={group.groupName}
                            data={group}
                            colSpan={7}
                            gradeOptions={this.props.gradeOptions}
                            onClickEditPA={this.onClickEditPA}
                            onQuickEditPA={this.onQuickEditPA}
                        />
                    );
                }));
        }
    };

    render() {
        return (
            <Fragment>
                <table className="table table-bordered" >
                    <thead>
                        <tr>
                            <th>{getResource('NickName')}</th>
                            <th>{getResource('Toeic')}</th>
                            <th>{getResource('Self')}</th>
                            <th>{getResource('ProjectManager')}</th>
                            <th>{getResource('Evaluator')}</th>
                            <th>{getResource('Reviewer')}</th>
                            <th>{getResource('Revised')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            this.renderRow()
                        }
                    </tbody>
                </table>
                {
                    this.state.isShowEditModal &&
                    <EditMemberPA
                        isShowModal={this.state.isShowEditModal}
                        paRecordId={this.state.paRecordId}
                        gradeOptions={this.props.gradeOptions}
                        onCloseModal={this.onCloseModal}
                        onSubmitSuccess={this.onEditSucess}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;